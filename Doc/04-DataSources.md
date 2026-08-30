# Data Sources

## Where the data came from

The dataset is Tbilisi's municipal bus network, captured around late 2022 and
saved to disk as flat files. The service has never talked to a live source:
import reads files only.

**No source URL is recorded anywhere in the repository.** A search of every
blob ever committed, across all of history including deleted files, turns up
only four external links: two geography references in
[StopHeader.cs](../BusTable.Core/Models/StopHeader.cs) and two licence URLs
inside vendored front-end libraries. The `DataTransferProviderService` that was
deleted in commit `170dd01` sounds like a downloader but is not one — it only
assembles DTOs. Whatever fetched the data was never part of this solution.

The source can still be identified from the artefacts themselves.

### The stop list is the Tbilisi Transport Company API

[StopItem](../BusTable.Core/Import/StopItem.cs) binds exactly these JSON fields:

```text
id (string) · code (string, nullable) · name · lat · lon
```

That is field-for-field the stop payload of the TTC transit API, whose
public base URL is:

```text
https://transit.ttc.com.ge/pis-gateway/api/v2
```

with `/stops`, `/routes`, `/plan`, `/stops/{id}/routes` and arrival-time
endpoints, and a `locale` query parameter. Two details corroborate the match
beyond the field names:

- The API addresses a stop as `1:<code>` — a prefixed composite string. That is
  why `StopItem.Id` is typed `string` while `Code` is a numeric string parsed to
  `int`, a distinction that would otherwise make no sense.
- The API's `locale` parameter is very likely the origin of the `Language`
  dimension that runs through every request DTO in this project. The contracts
  were shaped for a multi-locale import that was never wired up.

As of 2026-08-31 the host is still alive: an unauthenticated request returns
`403 Forbidden` rather than failing to resolve. The API gates on an `X-Api-Key`
header, so re-downloading would need a key.

### The schedules are external, the route list is not

The two file kinds have different origins, and their XML declarations give it
away:

| File | Declaration | Origin |
| --- | --- | --- |
| `data_schedule_*.xml` | `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>` | Java/JAXB marshaller — an external backend. |
| `data_route_list.xml` | `<?xml version="1.0" encoding="utf-8" ?>` with a UTF-8 BOM | .NET `XmlWriter` — written locally. |

The route list is not the operator's format at all: its structure
(`<RouteList Count="N">` with `<RouteNumber>`, `<LongName>`, `<StopA>`,
`<StopB>`) is exactly what
[RouteList.XContent](../BusTable.Core/Import/RouteList.cs) *emits*, `StopA` /
`StopB` being this codebase's own naming. It is a re-export produced by this
project — or by a scratch tool sharing its types — not a downloaded artefact.

The schedule documents, by contrast, are genuinely foreign: their structure
(`<Schedule>` / `<WeekdaySchedules>` / `<Stops>`) is *not* what
`RouteSchedule.XContent` writes, so that getter cannot round-trip the files its
own setter reads.

## What the service expects on disk

Configured in [appsettings.json](../BusTable.Service/appsettings.json):

```json
"ImportSource": {
    "Directory": "D:\\Work\\BusTable\\SourceData\\",
    "RouteListFileName": "routes.xml",
    "StopListFileName": "stops-k.json"
}
```

That directory is a stale Windows path from the original machine and does not
exist here. The service needs three kinds of file in it:

| File | Format | Content |
| --- | --- | --- |
| `routes.xml` | XML | The full route list. |
| `stops-k.json` | JSON | The full stop list with coordinates. |
| `*f1.xml` | XML | One timetable per route, forward direction. Discovered by glob, not by name. |

Only files matching `*f1.xml` are read. The `*f0.xml` (reverse-direction)
files are ignored by the importer even when present — see
[Status and Backlog](08-Status-and-Backlog.md).

## Format: route list

```xml
<RouteList Count="5">
    <Route>
        <RouteNumber>531</RouteNumber>
        <LongName>ხიზანიშვილის და ლესელიძისა  ქ. გადაკვეთა  - ფალიაშვილის ქ.</LongName>
        <StopA>PALIASHVILIS K.</StopA>
        <StopB>GLDANIS VI-VIII M/R</StopB>
    </Route>
    <!-- ... -->
</RouteList>
```

`LongName` is Georgian; `StopA` / `StopB` are transliterated terminus names.
The `Count` attribute is written on export but not verified on import.

## Format: schedule

One document per route and direction, named `data_schedule_<route>_f<0|1>.xml`:

```xml
<Schedule>
    <Forward>true</Forward>
    <RouteNumber>531</RouteNumber>
    <WeekdaySchedules>
        <FromDay>MONDAY</FromDay>
        <ToDay>FRIDAY</ToDay>
        <Stops>
            <ArriveTimes>7:00,7:20,7:40,8:00,...,22:00</ArriveTimes>
            <Forward>false</Forward>
            <HasBoard>false</HasBoard>
            <Lat>41.80434606266805</Lat>
            <Lon>44.82817232608796</Lon>
            <Name>"Gldanni, M/D-4, Konstantine Leselidze Street" - [20229]</Name>
            <Sequence>1</Sequence>
            <StopId>20229</StopId>
            <Type>bus</Type>
            <Virtual>false</Virtual>
        </Stops>
        <!-- one <Stops> per stop along the route -->
    </WeekdaySchedules>
</Schedule>
```

Notes on this format:

- The timetable is **per stop**, not per vehicle run: each stop lists every time
  a bus passes it. A journey is recovered by reading the same ordinal position
  across consecutive stops.
- **The source ships several `<WeekdaySchedules>` blocks and the importer reads
  only the first.** `RouteSchedule` calls `.Element("WeekdaySchedules")`, which
  returns one. Three of the five snapshot routes carry separate weekday,
  Saturday and Sunday timetables, and the weekend ones are silently dropped —
  see [Status and Backlog](08-Status-and-Backlog.md) for the measured loss.
- `HasBoard`, `Type`, `Virtual`, `FromDay`, `ToDay` and the inner `Forward` are
  parsed over but not carried into the model.
- A schedule can legitimately be empty. `data_schedule_387_f0.xml` in the test
  snapshot contains a `<Schedule>` with no stops at all.

## Format: stop list

Not present in the repository; its shape is fixed by
[StopItem](../BusTable.Core/Import/StopItem.cs) and matches the TTC API's
`/stops` response — a JSON array of objects:

```json
[
  { "id": "1:3375", "code": "3375", "name": "#1 Logopedic Kindergarten",
    "lat": 41.80539999998982, "lon": 44.82726800000056 }
]
```

`code` is parsed as an integer and becomes the stop's key; entries with code `0`
are skipped. `id` — the API's `1:<code>` composite — is read but unused. A
duplicate `code` in the file will throw during import.

## The surviving snapshot

[BusTable.Test/Resources/](../BusTable.Test/Resources/) holds a working extract:
one route list and ten schedule documents covering five Tbilisi routes.

| File | Size | Content |
| --- | --- | --- |
| `data_route_list.xml` | 1.5 KB | Routes 304, 310, 383, 387, 531 |
| `data_schedule_304_f0.xml` / `_f1.xml` | 116 / 118 KB | Gldani VII-VIII → State University |
| `data_schedule_310_f0.xml` / `_f1.xml` | 75 / 82 KB | Baratashvili St → State University |
| `data_schedule_383_f0.xml` / `_f1.xml` | 36 / 37 KB | State University → Baratashvili St |
| `data_schedule_387_f0.xml` / `_f1.xml` | 0.1 / 29 KB | Saburtalo–Vake (circular; `f0` is empty) |
| `data_schedule_531_f0.xml` / `_f1.xml` | 93 / 50 KB | Gldani VI-VIII → Paliashvili St |

These are embedded into the test assembly through
[Source.resx](../BusTable.Test/Resources/Source.resx) and loaded as strings by
`TestImportService`, so the tests need no files on disk and no configuration.

**There is no stop list in the snapshot.** `TestImportService.LoadStopRegistry`
returns an empty registry on purpose, and the stops are then discovered from the
schedule documents themselves — every `<Stops>` element carries its own
coordinates and name, which is exactly what `StopRegistry.AddStop` needs. This
is the fallback path that makes the snapshot self-sufficient.

## Running the service against the snapshot

The test resources can also feed the real service. Copy the XML files into a
directory, point `ImportSource:Directory` at it, and provide a `stops-k.json` —
either a real one, or `[]` if you are happy for stops to be discovered from the
schedules. See [Build, Run and Test](07-Build-Run-Test.md) for the exact steps
and for the caveat about stops discovered this way.
