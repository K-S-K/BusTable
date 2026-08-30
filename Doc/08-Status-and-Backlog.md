# Status and Backlog

Snapshot of the project as of August 2026. The last functional commit was in
March 2023; nothing has changed since except `.gitignore`.

## What works

- **Import pipeline.** Route list (XML), stop list (JSON) and per-route
  schedules (XML) parse into the in-memory model, cross-linked by stop code and
  route number, with routes that have no schedule pruned automatically.
- **Stop index.** `SortedDictionary` keyed by public stop code, O(log n) lookup,
  stable stop-code ordering for free.
- **Geo filtering.** Haversine great-circle distance, filter by radius in
  kilometres.
- **Five endpoints**, versioned by header, documented by Swagger, with search
  and pagination on the two collection endpoints.
- **Tests.** 6 MSTest cases, green, with the source data embedded as resources
  so they need no files and no configuration.
- **CI.** Builds, tests and publishes the service on every push to `master`.
- **Test UI.** A Razor Pages site that fetches the route list across CORS and
  renders it as a table. It exists to prove the API is browser-reachable and
  nothing more.

Verified while writing this documentation: `dotnet build` succeeds with 0 errors
on .NET SDK 10, `dotnet test` passes 6/6, and the service serves all five
endpoints against the test snapshot.

## Defects found

These were found by reading and then reproduced against the data in the
repository. None of them are covered by the existing tests.

### 1. A stop is dropped from the first route that introduces it

In `ImportService.ApplyRouteSchedule`
([ImportService.cs](../BusTable.Service/Services/ImportService.cs)):

```csharp
if (!routeService.TryGetStopById(input.StopId, out StopHeader? stopHeader))
{
    routeService.AddStop(input);
}

if (stopHeader == null)
{
    continue;          // ← skips the stop that was just registered
}
```

`TryGetStopById` sets `stopHeader` to `null` when the lookup fails. `AddStop`
registers the stop, but nothing re-reads it, so the `continue` drops it from the
route currently being imported. The stop is then present for every *later*
route, which is why the symptom is intermittent and why the tests never caught
it.

Measured against the test snapshot with an empty stop list:

| Route | Stops in the source file | Stops served |
| --- | --- | --- |
| 304 (imported first) | 56 | 0 |
| 531 (imported last) | 62 | 31 |

The bug is invisible whenever `stops-k.json` already contains every stop, which
is presumably why it survived. Fix: assign `stopHeader` from the newly added
stop, or have `AddStop` return it.

### 2. Default page size overflows on page 3

`RequestWithPagination.PageSize` defaults to `int.MaxValue`, and both registries
page with `Skip(PageSize * (PageNumber - 1))`. From page 3 onward the
multiplication overflows silently (`int.MaxValue * 2 == -2` unchecked), and
LINQ's `Skip` treats a negative count as zero:

| `PageNumber` (default page size) | Routes returned |
| --- | --- |
| 1 | 5 (all) |
| 2 | 0 |
| 3 | 5 (all) — should be 0 |
| 4 | 0 |

Fix: compute the skip in `long`, or treat "no page size" as a nullable rather
than `int.MaxValue`.

### 3. Coordinate parsing depends on the current culture

`RouteStop` parses latitude and longitude with `double.Parse(...)` and no
`IFormatProvider`, so the decimal separator follows the thread culture. Under a
comma-decimal culture the separator is discarded rather than rejected:

```text
under de-DE: StopId=20229 Lat=4180434606266805 Lon=4482817232608796
```

Every distance computed from such data is nonsense, and nothing throws. The
time parsing in the same class is already careful to pass
`CultureInfo.InvariantCulture`; the coordinate parsing was simply missed. The
JSON stop list is unaffected — `System.Text.Json` is invariant by design.

### 4. Weekend and Sunday timetables are silently discarded

`RouteSchedule` reads `value?.Element("WeekdaySchedules")` — singular. The
source documents carry one block per day band, and everything after the first
is dropped without a warning. Measured across the snapshot:

| File | Blocks in the source | Stop entries read | Discarded |
| --- | --- | --- | --- |
| 304 f1 | Mon–Fri, Sat, Sun | 56 | 112 |
| 304 f0 | Mon–Fri, Sat, Sun | 55 | 110 |
| 310 f1 | Mon–Fri, Sat, Sun | 32 | 64 |
| 310 f0 | Mon–Fri, Sat, Sun | 31 | 62 |
| 531 f0 | Mon–Fri, Sat–Sun | 57 | 57 |
| 383, 387 | Mon–Sun only | all | 0 |

The dropped blocks are real data, not duplicates. Route 304's first stop departs
at `7:42, 8:14, 8:30, 8:46 …` on weekdays but `7:50, 8:10, 8:30, 8:50 …` at
weekends — a different headway entirely. Every departure time this API serves is
therefore a **weekday** time, presented without qualification.

This one is not a one-line fix. Each block repeats the same stop ids in the same
order, and `RouteSchedule.RouteStops` is a `Dictionary<int, RouteStop>` filled
with `.Add()` — so simply switching to `.Elements(...)` would throw on the first
duplicate key. Supporting day bands needs a model that keys schedules by
(day band, stop) or hangs a list of timetables off each stop.

### 5. Nearest-stop results are not sorted by distance

`/BusStops` returns stops in stop-code order, so paging a distance-filtered
query does not give you the nearest stops first. See
[Geo and Search](06-Geo-Search.md).

## Known limitations (by design or simply unfinished)

- **One direction only.** `ImportService` globs `*f1.xml` and ignores every
  `*f0.xml`, so the reverse direction of every route is invisible to the API,
  even though the data is on disk. This is the substance of open issue #19.
- **The language dimension is decorative.** Every request carries `Language`,
  every response echoes it, everything is imported as `ANY`, and
  `LanguageValidationService` is registered in DI but never called — the
  validation calls in the controllers are commented out.
- **`CityId` is ignored** on all three endpoints that accept it.
- **No authentication.** Each controller contains a commented-out
  `User.Identity.IsAuthenticated` placeholder; `UseAuthorization` is in the
  pipeline with no authentication scheme behind it.
- **No persistence and no reload.** The dataset is rebuilt from files in the
  `RouteService` constructor; refreshing the data means restarting the process.
- **Import happens on first request**, inside a constructor, so the first caller
  pays the full parse cost and any data error surfaces as a 500 rather than a
  start-up failure.
- **The stale import path.** `appsettings.json` still points at
  `D:\Work\BusTable\SourceData\`, and no source data is committed.
- **CI drift.** The workflow pins .NET 6 while the projects target .NET 7, and
  uses superseded action versions. See [Build, Run and Test](07-Build-Run-Test.md).
- **`net7.0` is out of support** since May 2024.

## Open issues in the tracker

| # | Title | Note |
| --- | --- | --- |
| 19 | Add integer IDs for Routes | Body: "It is necessary to deal with bi-directional routes (back routes) schedules." Route numbers are strings today, and the reverse direction is not imported at all. |
| 25 | Refactoring: ScheduleRegistry | `ScheduleRegistry` is a bare dictionary with one method; it has none of the encapsulation the other two registries got. |

## If the project is picked up again

Roughly in order of value per effort:

1. **Fix the defects above** and add tests that would have caught them —
   in particular an import test that starts from an empty stop registry.
2. **Retarget to a supported framework** (.NET 8 LTS or later) and update the CI
   workflow to match.
3. **Import both directions and all day bands.** The `f0` files and the weekend
   blocks are already on disk; together they are the largest gap between the
   data the project holds and the data it serves.
4. **Sort geo results by distance**, and add a spatial index (grid or geohash
   buckets) so the search stops being an O(n) scan.
5. **Move the import out of the constructor** into an explicit hosted service or
   start-up step, so failures are loud and the data can be reloaded.
6. **Commit a small dataset** so the service runs out of the box — the test
   snapshot is nearly enough already. Refreshing it from the live source is also
   still possible in principle: the TTC transit API identified in
   [Data Sources](04-DataSources.md) still responds, though it now requires an
   `X-Api-Key`.
7. **Decide the fate of the UI.** It is a scaffold with one hardcoded fetch. It
   is either worth a real map view (stops around a point, route overlay) or
   worth deleting; keeping it as-is serves nobody.
