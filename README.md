# BusTable

[![.NET](https://github.com/K-S-K/BusTable/actions/workflows/dotnet.yml/badge.svg)](https://github.com/K-S-K/BusTable/actions/workflows/dotnet.yml)

An experiment in serving a city's public transport timetable entirely from
memory: load the operator's raw XML and JSON dumps once at start-up, cross-link
routes to stops to schedules, index them, and answer passenger questions over a
REST API — no database, no ORM, no cache.

The dataset is Tbilisi's municipal bus network.

## The idea

Transport operators publish data shaped around their own concerns: a route list,
a stop list, and one timetable document per route. Passengers ask different
questions:

- Which stops are within walking distance of me?
- Which stops does route 531 call at?
- When does the next 531 leave from *this* stop?

A city fits in RAM — a few thousand stops, a few hundred routes. So the whole
dataset is parsed into an in-memory read model at start-up, and every query
becomes a dictionary lookup or a single pass over a list.

## What is implemented

- **Import pipeline** — XML route lists and schedules, JSON stop lists, parsed
  into a domain model and cross-linked; routes without a schedule are pruned.
- **Stop index** — a `SortedDictionary` keyed by public stop code, giving
  O(log n) resolution during import and a stable ordering for free.
- **Geo search** — stops within *N* kilometres of a coordinate, using the
  haversine great-circle distance.
- **REST API** — five endpoints, versioned by header, documented with Swagger,
  with search and pagination.
- **Tests** — 6 MSTest cases running against a source-data snapshot embedded in
  the test assembly, so they need no files and no configuration.
- **Test UI** — a minimal Razor Pages site, used only to prove the API is
  reachable from a browser across CORS.

## Layout

| Project | Role |
| --- | --- |
| `BusTable.Core` | Domain model, DTOs and import parsers. No external dependencies. |
| `BusTable.Service` | ASP.NET Core Web API, import orchestration, DI composition root. |
| `BusTable.Site` | Razor Pages test UI. Talks to the API over HTTP. |
| `BusTable.Test` | MSTest + Moq, with the data snapshot embedded as resources. |

```text
BusTable.Test ──► BusTable.Service ──► BusTable.Core
BusTable.Site ····(HTTP + CORS)····► BusTable.Service
```

## API at a glance

| Endpoint | Returns |
| --- | --- |
| `GET /Languages` | Supported language codes. |
| `GET /BusRoutes` | Routes, with optional text search and paging. |
| `GET /BusStops` | Stops near a coordinate, with the distance in kilometres. |
| `GET /BusRouteStops` | The ordered stops of one route. |
| `GET /BusDepartureTimes` | Every departure of one route from one stop. |

Versioning is by the `BusTable-API-Version` header. Full request and response
shapes are in [Doc/05-API.md](Doc/05-API.md).

## Build and run

```bash
dotnet build
dotnet test
```

The service imports its data from a directory configured under `ImportSource` in
`appsettings.json`. No source data is committed, and the configured path is a
stale one from the original machine, so point the importer somewhere real before
running:

```bash
cd BusTable.Service
ImportSource__Directory=/path/to/data/ dotnet run
```

Swagger is then at `https://localhost:7212/swagger`.
[Doc/07-Build-Run-Test.md](Doc/07-Build-Run-Test.md) shows how to build a
working data directory out of the test snapshot in the repository.

## Status

This is a 2022–2023 experiment, revisited in 2026 to document it. It builds
clean on the current .NET SDK and all tests pass, but it targets the
out-of-support `net7.0`, imports only one direction of each route and only the
weekday half of each timetable, and carries several reproduced defects. All of that is written down honestly in
[Doc/08-Status-and-Backlog.md](Doc/08-Status-and-Backlog.md).

## Documentation

Full documentation is in [Doc/](Doc/README.md):

- [Overview](Doc/01-Overview.md) — the idea and the scope
- [Architecture](Doc/02-Architecture.md) — projects, layers, start-up flow
- [Data Model](Doc/03-DataModel.md) — import types, read model, registries
- [Data Sources](Doc/04-DataSources.md) — file formats and the surviving snapshot
- [API](Doc/05-API.md) — endpoints and response examples
- [Geo and Search](Doc/06-Geo-Search.md) — indexing and distance calculation
- [Build, Run and Test](Doc/07-Build-Run-Test.md) — getting it running
- [Status and Backlog](Doc/08-Status-and-Backlog.md) — state, defects, next steps

## License

[MIT](LICENSE.txt)
