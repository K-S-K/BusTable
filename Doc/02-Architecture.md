# Architecture

## Solution layout

Four projects, all targeting `net7.0`:

| Project | SDK | Role |
| --- | --- | --- |
| `BusTable.Core` | `Microsoft.NET.Sdk` | Domain model, DTOs, import parsers. No dependencies at all — not even on ASP.NET. |
| `BusTable.Service` | `Microsoft.NET.Sdk.Web` | REST API, import orchestration, DI composition root. |
| `BusTable.Site` | `Microsoft.NET.Sdk.Web` | Razor Pages test UI. Standalone — it does not reference the other projects, it calls the API over HTTP. |
| `BusTable.Test` | `Microsoft.NET.Sdk` | MSTest + Moq unit tests, with a snapshot of the source data embedded as resources. |

Dependency direction:

```text
BusTable.Test ──► BusTable.Service ──► BusTable.Core
BusTable.Site ····(HTTP + CORS)····► BusTable.Service
```

`BusTable.Core` deliberately knows nothing about hosting, configuration or
logging. Everything that touches the file system or the outside world lives in
`BusTable.Service`.

## Layers inside Core

```text
BusTable.Core
├── Import/   parsers for the operator's file formats (XML, JSON)
├── Models/   the in-memory read model + the registries that hold it
├── Dto/      request and response contracts of the API
└── Common/   BadRequestException
```

The separation between `Import` and `Models` is the point of the design: the
import types mirror the *source files* (including their quirks), the model types
mirror what the *service* wants to serve. `ImportService` is the only place
where one is translated into the other.

## Runtime composition

Everything is registered as a singleton in
[Program.cs](../BusTable.Service/Program.cs):

```csharp
builder.Services.AddSingleton<LanguageValidationService>();
builder.Services.AddSingleton<RouteService>();
builder.Services.AddSingleton<IImportService, ImportService>(
    sp => new ImportService(importSourceSettings));
```

`ImportSourceSettings` is bound from the `ImportSource` section of
`appsettings.json` at start-up and passed into `ImportService` by hand rather
than through the options pattern.

The pipeline is minimal: Swagger (development only), HSTS (otherwise), HTTPS
redirection, authorization (no authentication is configured), CORS, controllers.

## Start-up: how the data gets loaded

There is no explicit "load the data" step. The whole import happens inside the
`RouteService` **constructor**, which the DI container runs the first time any
controller asks for it:

```text
RouteService(IImportService)
│
├─ 1. stopRegistry     = LoadStopRegistry()
│        reads  <Directory>/<StopListFileName>   (stops-k.json)
│        builds a SortedDictionary<int, StopHeader> keyed by stop code
│
├─ 2. routeRegistry    = LoadRouteRegistry()
│        reads  <Directory>/<RouteListFileName>  (routes.xml)
│        builds a Dictionary<string, BusRouteItem> keyed by route number
│
├─ 3. scheduleRegistry = LoadScheduleRegistry(routeRegistry.Items.Keys, this)
│        enumerates <Directory>/*f1.xml          (one file per route, forward direction)
│        for each: parse RouteSchedule → map its stops against stopRegistry,
│                  registering stops that the JSON did not contain
│        keeps only the schedules whose route number is in routeRegistry
│
└─ 4. prune: every route in routeRegistry with no schedule is removed
```

Two consequences worth knowing:

- **Routes and schedules must agree.** A route present in `routes.xml` but with
  no `*f1.xml` file is dropped from the API entirely. This is what keeps the
  five-route test snapshot self-consistent.
- **The import is circular by design.** `LoadScheduleRegistry` takes the
  half-built `RouteService` back as a parameter so that schedule parsing can
  call `TryGetStopById` / `AddStop` on the stop registry. It works, but it is
  the least pleasant part of the design — a schedule parser should not need a
  service reference. See [Status and Backlog](08-Status-and-Backlog.md).

Because everything is a singleton and the constructor does the work, the first
request pays the whole import cost and every request after that is served purely
from memory.

## Request flow

```text
HTTP GET /BusStops?Lat=..&Lon=..&MaxDistance=..
   │
   ▼
BusStopsController          model-binds the query string into BusStopsRequest
   │                        catches BadRequestException → 400, other → 500
   ▼
RouteService.GetStops       wraps the result in the response DTO
   │
   ▼
StopRegistry.GetStops       computes distances, filters, paginates
```

All four data controllers follow exactly the same shape: bind a request DTO
from the query string, call one `RouteService` method, translate exceptions into
status codes, return `404` when the service returns `null`.

`RouteService` is the single façade over the three registries; controllers never
touch a registry directly.

## Things that are wired but not used

- `LanguageValidationService` is registered in DI and never injected. The calls
  to it inside the controllers are commented out.
- The authentication checks in every controller are commented-out placeholders
  (`if (!User.Identity.IsAuthenticated) return Forbidden();`).
- `CityId` is bound on three request DTOs and never read.
- `Language` is bound everywhere, echoed back in responses, and never used to
  select data — the import tags everything `ANY`.
