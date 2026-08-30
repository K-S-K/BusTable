# API

A REST API over the in-memory read model. Five controllers, one `GET` each, all
parameters bound from the query string.

Swagger UI is served at `/swagger` in the Development environment. Default local
address is `https://localhost:7212` (see
[launchSettings.json](../BusTable.Service/Properties/launchSettings.json)).

## Versioning

Versioning is by **header**, not URL:

```text
BusTable-API-Version: 1.0
```

Configured in [Program.cs](../BusTable.Service/Program.cs) with
`HeaderApiVersionReader`. `AssumeDefaultVersionWhenUnspecified` is on, so a
request without the header is treated as `1.0`; responses advertise the
supported versions via `api-supported-versions`.

## Common parameters

| Parameter | Applies to | Meaning |
| --- | --- | --- |
| `Language` | all | Accepted and echoed back, but not used for filtering. Always `ANY` in practice. |
| `CityId` | routes, route stops, departures | Accepted and ignored. Placeholder for multi-city support. |
| `PageNumber` | routes, stops | 1-based, clamped to a minimum of 1. |
| `PageSize` | routes, stops | Clamped to a minimum of 3. Defaults to `int.MaxValue`, meaning "no paging". |

## Common responses

| Status | When |
| --- | --- |
| `200` | Data found. |
| `400` | The service threw `BadRequestException`. |
| `404` | The service returned `null` — unknown route number or unknown stop on that route. |
| `500` | Any other exception, returned as a ProblemDetails body. |

Non-ASCII text is escaped by the default JSON encoder, so Georgian route names
come back as `\uXXXX` sequences. That is valid JSON and decodes correctly in any
client; it is simply not human-readable in a raw response dump.

## `GET /Languages`

The list of language codes the API claims to support. Currently hardcoded in
[LanguageData](../BusTable.Core/Dto/LanguageData.cs):

```json
["ANY", "EN", "GE", "RU"]
```

## `GET /BusRoutes`

All routes, with an optional case-insensitive substring search over the long
name and both terminus names.

Parameters: `Search`, `PageNumber`, `PageSize`, `CityId`, `Language`.

```json
{
  "count": 3,
  "pageNumber": 1,
  "language": "ANY",
  "items": [
    {
      "number": "304",
      "name": "გლდანის VII - VIII ...",
      "stop1": "State University H/B",
      "stop2": "Gldani VII-VIII M/D",
      "circle": false
    }
  ]
}
```

`circle` is computed, not stored: it is true when both terminus names are equal.

## `GET /BusStops`

Stops near a coordinate. This is the geo-search endpoint — see
[Geo and Search](06-Geo-Search.md) for how the distance is computed and what the
ordering guarantees are (there are none).

Parameters: `Lat`, `Lon`, `MaxDistance` (kilometres), `PageNumber`, `PageSize`,
`Language`.

Filtering is applied only when `Lat > 1`, `Lon > 1` **and** `MaxDistance > 0.1`;
otherwise every stop is returned, still carrying a computed `distance`.

```json
{
  "count": 3,
  "pageNumber": 1,
  "language": "ANY",
  "items": [
    { "id": 1942, "name": "Khergiani Street",
      "lon": 44.82394700000057, "lat": 41.80682799998958, "distance": 0.31805131424208727 },
    { "id": 3050, "name": "#1 Logopedic Kindergarten",
      "lon": 44.82691400000057, "lat": 41.80538699998983, "distance": 0.029402651806855137 },
    { "id": 3375, "name": "#1 Logopedic Kindergarten",
      "lon": 44.82726800000056, "lat": 41.80539999998982, "distance": 0 }
  ]
}
```

`distance` is in kilometres from the requested point. Note in the example above
that the results are **not** ordered by distance.

## `GET /BusRouteStops`

The ordered list of stops along one route.

Parameters: `RouteNumber` (required), `CityId`, `Language`.

```json
{
  "language": "ANY",
  "routeNumber": "531",
  "items": [
    { "id": 20229, "name": "Gldanni, M/D-4, Konstantine Leselidze Street",
      "lon": 44.82817232608796, "lat": 41.80434606266805 },
    { "id": 3375, "name": "#1 Logopedic Kindergarten",
      "lon": 44.82726800000056, "lat": 41.80539999998982 }
  ]
}
```

Arrival times are held on these items internally but are `[JsonIgnore]`d — ask
`/BusDepartureTimes` for them. Returns `404` for an unknown route number.

## `GET /BusDepartureTimes`

Every departure of one route from one stop.

Parameters: `RouteNumber`, `StopID`, `CityId`, `Language`.

```json
{
  "stopId": 3899,
  "stopName": "Abashidze St 78",
  "language": "ANY",
  "count": 63,
  "times": [
    { "departure": "08:25" },
    { "departure": "08:45" },
    { "departure": "09:05" }
  ]
}
```

Times are sorted ascending as strings of a `TimeSpan`, which means departures
after midnight (`24:15` in the source, normalized to `00:15`) appear at the
*start* of the list rather than the end.

Returns `404` if the route is unknown, or if the route is known but does not
call at that stop.

## CORS

The default policy allows exactly one origin, `https://localhost:7179` — the
Razor Pages test site — and the `BusTable-API-Version` header. Any other origin
is refused.
