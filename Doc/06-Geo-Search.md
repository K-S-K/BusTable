# Geo and Search

Two lookup mechanisms exist in the model: an exact index for stop codes, and a
distance scan for coordinates.

## The stop index

[StopRegistry](../BusTable.Core/Models/StopRegistry.cs) keeps every stop in a
`SortedDictionary<int, StopHeader>` keyed by the public stop code:

```csharp
private readonly SortedDictionary<int, StopHeader> _ixCode = new();

public bool TryGetById(int code, out StopHeader? item) => _ixCode.TryGetValue(code, out item);
```

This is the hot path of the whole import. Every `<Stops>` element in every
schedule document names a stop by id, and every one of them is resolved through
`TryGetById` — for a city-sized dataset that is hundreds of thousands of
lookups, each O(log n).

A `SortedDictionary` (red-black tree) rather than a `Dictionary` gives ordered
enumeration for free, which is what produces the public `Stops` list:

```csharp
Stops = _ixCode.Values.ToList();
```

So `Stops` is always in ascending stop-code order — a stable, reproducible order
that makes paging deterministic, though it has no relationship to geography.

The trade-off is deliberate but worth naming: a plain `Dictionary` would give
O(1) lookups, and the ordering could be produced once at the end of import. The
sorted structure pays a log factor on every lookup to keep the invariant always
true.

## Distance between two points

[StopHeader.GetDistance](../BusTable.Core/Models/StopHeader.cs) computes the
**great-circle distance** using the haversine formula:

```text
a = sin²(Δφ/2) + cos φ₁ · cos φ₂ · sin²(Δλ/2)
c = 2 · asin(√a)
d = R · c
```

with `R = 6376.5 km`.

**This is a spherical model, not a geoid.** The Earth is treated as a perfect
sphere. It is not an ellipsoidal (WGS-84) geodesic calculation, and it does not
consult a geoid model for elevation.

For "which stops are within walking distance", that is the right tool: at city
scale the error against a proper geodesic is a few metres, far below the
accuracy of the stop coordinates themselves.

## About that radius

`6376.5` is not the standard mean radius (6371.0 km), which invites the question
of whether it was chosen for Tbilisi's latitude. It was not.

The constant is inherited. The entire block — the two reference URLs, the
formula, and the commented-out sibling `kEarthRadiusMiles = 3956.0` — arrived in
a single commit (`ef6aadd`), and the `k`-prefixed naming is not this codebase's
style. It is a verbatim paste from the cited CodeProject article.

The pair is also internally inconsistent: 3956 miles is 6366.56 km, which
disagrees with 6376.5 km by 9.94 km. A deliberately localized radius would not
ship next to a sibling constant that is ten kilometres away from it.

What makes the question a fair one is that the constant is *accidentally* well
suited to this city. There is no single "radius at a latitude", and for a
spherical approximation of short distances the appropriate one is the Gaussian
mean radius of curvature, √(M·N):

| Value at 41.8° N (Tbilisi) | km | vs. 6376.5 |
| --- | --- | --- |
| Geocentric radius | 6368.68 | −7.8 km |
| Meridional curvature *M* | 6363.81 | −12.7 km |
| Prime-vertical curvature *N* | 6387.64 | +11.1 km |
| **Gaussian mean √(M·N)** | **6375.72** | **−0.78 km (0.012 %)** |
| Gaussian mean + ~450 m elevation | 6376.17 | −0.33 km |
| IUGG mean radius | 6371.01 | −5.5 km (0.086 %) |

So the inherited value is roughly six times closer to the locally correct radius
than the textbook 6371 km would have been — 6 cm of error on a 500 m walk rather
than 37 cm. Both are far below the noise floor of the data.

It remains a coincidence. The Gaussian radius rises monotonically from
6356.8 km at the equator to 6399.6 km at the pole, so some latitude matches any
constant in that band; 6376.5 km corresponds to 42.86° N, about a degree north
of Tbilisi.

None of this is worth "fixing" for accuracy — but if the calculation is ever
touched, the constant should be named and sourced rather than left as a bare
literal.

The implementation uses `2·asin(√a)` rather than the more common
`2·atan2(√a, √(1−a))`. The two are equivalent for the distances involved here;
the `atan2` form is only better conditioned for near-antipodal points.

## Nearest-stop search

`StopRegistry.GetStops(BusStopsRequest)` is a straight linear pass:

```csharp
IQueryable<BusStopHeader> items = Stops.Select(x => new BusStopHeader
{
    Id = x.Id, Name = x.Name, Lat = x.Lat, Lon = x.Lon,
    Distance = x.GetDistance(request.Lat, request.Lon)
}).AsQueryable();

if (request.DistanceCencitive)
    items = items.Where(x => x.Distance <= request.MaxDistance);

items = items.Skip(request.PageSize * (request.PageNumber - 1)).Take(request.PageSize);
```

Behaviour worth knowing:

1. **The distance is always computed**, for every stop, even when the request is
   not distance-sensitive. `BusStopHeader.Distance` is always populated.
2. **Filtering is opt-in through a guard**, not a flag. `DistanceCencitive`
   *(spelling as in the source)* is true only when `Lat > 1 && Lon > 1 &&
   MaxDistance > 0.1`. A request with no coordinates therefore returns the whole
   city rather than an empty result.
3. **Results are not sorted by distance.** They come back in stop-code order.
   "The three nearest stops" is not what `PageSize=3` returns — it returns three
   arbitrary stops that happen to be within range. This is the single most
   surprising thing about the endpoint.
4. **It is O(n) per request.** Fine for a few thousand stops; the natural next
   step for a real workload is a spatial index (geohash buckets, a grid, or a
   k-d tree) so that only nearby candidates are measured.
5. The default coordinates on `BusStopsRequest` (41.8054 N, 44.8273 E) are not
   arbitrary — they are the position of Tbilisi stop 3375, so an unparameterized
   Swagger call returns something meaningful.

## Route text search

`RouteRegistry.GetRoutes` is the other search path, and it is intentionally
naive: `ToLower()` on both sides and `Contains` against the long name and the
two terminus names, then page. No normalization, no transliteration, no
tolerance for typos.

Because route long names are in Georgian while terminus names are transliterated
Latin, a search for `"Univer"` matches the Latin fields only — which is exactly
what the `RoutesFilteringTest_Univer` test asserts.
