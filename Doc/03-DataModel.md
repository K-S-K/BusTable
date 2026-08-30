# Data Model

Three groups of types, in three folders of `BusTable.Core`.

## Import types — shaped like the source files

These mirror the operator's file formats one-to-one, quirks included. Each XML
type exposes an `XContent` property that both reads and writes the element, so a
type can round-trip its own format.

| Type | Source | Notes |
| --- | --- | --- |
| [RouteList](../BusTable.Core/Import/RouteList.cs) | `routes.xml` | `Dictionary<string, RouteItem>` keyed by route number. Routes numbered `"0"` are skipped. |
| [RouteItem](../BusTable.Core/Import/RouteItem.cs) | `<Route>` | Route number, long name (Georgian), and the two terminus stop names. |
| [RouteSchedule](../BusTable.Core/Import/RouteSchedule.cs) | one `data_schedule_<route>_f<0\|1>.xml` | Route number, direction flag, and `Dictionary<int, RouteStop>` keyed by stop id. |
| [RouteStop](../BusTable.Core/Import/RouteStop.cs) | `<Stops>` | One stop on one route: coordinates, sequence number, name, and the full list of arrival times. |
| [StopItem](../BusTable.Core/Import/StopItem.cs) | `stops-k.json` | JSON-attributed: `id`, `code`, `name`, `lat`, `lon`. |

Two parsing details that matter:

- **After-midnight times.** The source writes times past midnight as `24:15`,
  `25:30`, `26:05`. `RouteStop` rewrites the hour to `0:`, `1:`, `2:` before
  parsing, then sorts the list ascending — so post-midnight departures end up at
  the *front* of the list, not the end.
- **Names carry their own id.** Stop names arrive as
  `"Khergiani Street" - [3376]`. `RouteStop` strips the ` - [id]` suffix and the
  surrounding quotes.
- **Two ids per stop.** `StopItem` has both an `id` (the operator's internal
  identifier) and a `code` (the number printed on the stop sign). The model uses
  `code` as its key — `id` is parsed but discarded.

## Model types — the in-memory read model

| Type | Purpose |
| --- | --- |
| [StopHeader](../BusTable.Core/Models/StopHeader.cs) | A stop: `Id` (stop code), `Name`, `Lat`, `Lon`. Also carries `GetDistance(lat, lon)` — see [Geo and Search](06-Geo-Search.md). |
| [StopInfo](../BusTable.Core/Models/StopInfo.cs) | `StopHeader` plus the arrival times of one route at that stop. The times are `[JsonIgnore]`d, so they never leak into the route-stops response. |
| [BusRouteItem](../BusTable.Core/Models/BusRouteItem.cs) | A route: number, long name, both termini, and `Circle` (true when the two termini are the same name). |
| [BusDepartureTimeItem](../BusTable.Core/Models/BusDepartureTimeItem.cs) | One departure. Holds a `TimeSpan` internally, serializes only as `"hh:mm"`. |

## Registries — the three tables of the read model

### StopRegistry

[StopRegistry](../BusTable.Core/Models/StopRegistry.cs) — every stop in the
city, held twice:

- `SortedDictionary<int, StopHeader> _ixCode` — the index, keyed by stop code.
  Backs `TryGetById`, which is how schedules resolve their stops during import.
- `List<StopHeader> Stops` — a flat list for scanning, in stop-code order
  (it is materialized from the sorted dictionary's values).

It owns `Load(fileName)` for the JSON dump, `AddStop(RouteStop)` for stops that
only appear inside a schedule, and `GetStops(BusStopsRequest)` for the
geo-search query.

`AddStop` throws if the code is already present — the registry refuses to merge
two different records for the same stop rather than silently picking one.

### RouteRegistry

[RouteRegistry](../BusTable.Core/Models/RouteRegistry.cs) —
`Dictionary<string, BusRouteItem>` keyed by route number, plus
`GetRoutes(BusRoutesRequest)`, which does a case-insensitive `Contains` search
over the long name and both terminus names, then paginates.

Route numbers are strings, not integers, because the source treats them as
labels. Making them integers is open issue #19.

### ScheduleRegistry

[ScheduleRegistry](../BusTable.Core/Models/ScheduleRegistry.cs) —
`Dictionary<string, StopRouteSchedule>` keyed by route number. It is the
thinnest of the three: a dictionary and a `TryGetValue`. Open issue #25 is about
giving it a real shape.

Each `StopRouteSchedule` is the ordered list of `StopInfo` for one route, each
carrying that route's arrival times at that stop. This is the join table of the
model: routes → stops → times.

## DTOs — the API contracts

Requests form a small inheritance chain:

```text
RequestWithLanguage            Language (default "ANY")
└── RequestWithPagination      PageNumber, PageSize (clamped: min page size 3)
    ├── BusRoutesRequest       CityId, Search
    └── BusStopsRequest        Lat, Lon, MaxDistance
RequestWithLanguage
└── BusRouteStopsRequest       CityId, RouteNumber          ─┐ both implement
└── BusDepartureTimesForTheStopRequest  CityId, RouteNumber, StopID  ─┘ IBusRouteStopsRequest
```

`IBusRouteStopsRequest` exists so that `RouteService.GetRouteStops` can serve
both the "stops of a route" query and the first half of the "departures at a
stop" query from one code path.

`PageSize` defaults to `int.MaxValue` (meaning "no paging") and is clamped to a
minimum of 3. `PageNumber` is clamped to a minimum of 1.

Responses are equally flat: `BusRouteData`, `BusStopMetadata`,
`StopRouteSchedule` and `BusDepartureTimeData` each carry a `Language`, a
computed `Count`, and a list of items. See [API](05-API.md) for their JSON shapes.
