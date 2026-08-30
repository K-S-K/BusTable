# Build, Run and Test

## Prerequisites

The projects target `net7.0`, which is out of support. A current SDK still
builds them — verified here with .NET SDK 10.0.100 on macOS (arm64), which
restores the `net7.0` reference assemblies automatically and emits only
`NETSDK1138` "out of support" warnings.

## Build

```bash
dotnet build
```

Expected result: **Build succeeded, 0 errors**, with warnings only —
`NETSDK1138` (EOL framework), `MSB3539` (see below), and two nullability
warnings (`CS8604` in `Program.cs`, `CS8625` in `TestImportService.cs`).

Note that `BusTable.Core` and `BusTable.Service` set

```xml
<BaseOutputPath>..\..\Bin\</BaseOutputPath>
<BaseIntermediateOutputPath>..\..\Swp\Core</BaseIntermediateOutputPath>
```

so their build output lands **two directories above the repository root**, not
in the usual per-project `bin/`. `BusTable.Site` and `BusTable.Test` do not do
this and build normally. Setting `BaseIntermediateOutputPath` inside the
`.csproj` is also what triggers `MSB3539` — the supported way is a
`Directory.Build.props`.

## Test

```bash
dotnet test
```

Expected result: **6 passed, 0 failed** — two in `LanguagesControllerTest`, four
in `RouteServiceTest`.

The tests need no configuration and no files on disk: the source data is
embedded in the test assembly through
[Source.resx](../BusTable.Test/Resources/Source.resx) and fed in by
[TestImportService](../BusTable.Test/TestImportService.cs).

`TestImportService` substitutes for `ImportService` through a detail worth
noticing: it derives from `ImportService` but declares its methods with `new`
rather than `override` (the base methods are not virtual). It works because
`RouteService` depends on `IImportService`, and C# interface mapping picks the
most-derived implementation — so the interface calls land on the test methods
while `ApplyRouteSchedule` is still inherited from the real importer. It is
clever, but it is a trap: calling those methods through an `ImportService`-typed
reference would silently run the production code path.

## Run the API

The service needs a data directory before it can start — `RouteService` imports
everything in its constructor, and a missing directory throws on the first
request.

Since no source data ships with the repository, the fastest way to get a running
service is to build a data directory out of the test snapshot. The following was
used to produce the example responses in [API](05-API.md):

```bash
# 1. a working directory with the schedules and the route list
mkdir -p /tmp/SourceData
cp BusTable.Test/Resources/data_schedule_*.xml /tmp/SourceData/
cp BusTable.Test/Resources/data_route_list.xml /tmp/SourceData/routes.xml

# 2. a stop list; see below for why an empty one is not enough
#    (build it from the coordinates carried by the schedule documents)

# 3. run, pointing the importer at the directory
cd BusTable.Service
ImportSource__Directory=/tmp/SourceData/ dotnet run
```

The `ImportSource__Directory` environment variable overrides the stale Windows
path in [appsettings.json](../BusTable.Service/appsettings.json) without editing
the file. The double underscore is the .NET configuration convention for section
nesting.

Then open `https://localhost:7212/swagger`. On a first run you may need
`dotnet dev-certs https --trust`.

### Why the stop list matters

You can run with `stops-k.json` containing just `[]` — stops will be discovered
from the coordinates inside the schedule documents — but the result is wrong,
and measurably so. With an empty stop list:

| Route | Stops in the source file | Stops served by the API |
| --- | --- | --- |
| 304 | 56 | 0 |
| 531 | 62 | 31 |

With a complete stop list, both routes serve all of their stops. The cause is a
defect in `ImportService.ApplyRouteSchedule`, described in
[Status and Backlog](08-Status-and-Backlog.md) — a stop being seen for the first
time is registered but omitted from the route that introduced it.

To generate a usable stop list from the snapshot, extract `StopId`, `Name`,
`Lat` and `Lon` from every `<Stops>` element across the schedule files and write
them as the JSON array described in [Data Sources](04-DataSources.md).

## Run the test UI

```bash
cd BusTable.Site
dotnet run
```

It starts on `https://localhost:7179`, which is the one origin the API's CORS
policy allows. The API address is hardcoded in
[Index.cshtml](../BusTable.Site/Pages/Index.cshtml) as
`https://localhost:7212/BusRoutes`, so **the service must be running on its
default port** or the page shows an alert. The page renders one table of routes
and nothing else.

## Continuous integration

[.github/workflows/dotnet.yml](../.github/workflows/dotnet.yml) restores,
builds, tests and publishes the service on every push and pull request to
`master`.

It has drifted from the code: it pins `dotnet-version: 6.0.x` while the projects
target `net7.0` (the migration in issue #39 did not update the workflow), and it
uses `actions/checkout@v3` and `actions/upload-artifact@v3`, both superseded.
The README badge is labelled "GitHub Pages" but points at this .NET workflow.
