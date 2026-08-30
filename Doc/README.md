# BusTable Documentation

Documentation for the BusTable experiment: a small in-memory service that turns
a city's published bus timetable dumps into a queryable REST API.

Start with the [Overview](01-Overview.md) for the idea, or with
[Status and Backlog](08-Status-and-Backlog.md) for the honest state of things.

## Contents

| Document | What it covers |
| --- | --- |
| [01 — Overview](01-Overview.md) | The idea behind the experiment, its scope, and what it deliberately is not. |
| [02 — Architecture](02-Architecture.md) | The four projects, layering, DI composition, and how the data gets loaded at start-up. |
| [03 — Data Model](03-DataModel.md) | Import types, the in-memory read model, the three registries, and the API DTOs. |
| [04 — Data Sources](04-DataSources.md) | Where the data came from, the file formats, and the snapshot that survives in the repository. |
| [05 — API](05-API.md) | The five endpoints, versioning, paging, and real response examples. |
| [06 — Geo and Search](06-Geo-Search.md) | The stop index, the haversine distance calculation, and the nearest-stop search. |
| [07 — Build, Run and Test](07-Build-Run-Test.md) | Building, testing, and getting a running service out of the test snapshot. |
| [08 — Status and Backlog](08-Status-and-Backlog.md) | What works, the defects found and reproduced, known limitations, and what to do next. |

## Conventions

Code references link to the file in the repository. Anything stated as measured
in these documents was actually run against the code, not inferred — including
the defect reproductions in
[Status and Backlog](08-Status-and-Backlog.md).
