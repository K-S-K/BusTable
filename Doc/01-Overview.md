# Overview

## The idea

BusTable is an experiment in turning a city's raw public-transport timetable
data into a small, self-contained query service.

Public transport operators publish their data as bulk dumps: a list of routes,
a list of stops with coordinates, and one timetable document per route and
direction. Those dumps answer the questions the operator cares about
("what does route 531 look like?"), but not the questions a passenger has:

- Which stops are near me right now?
- Which routes serve this stop?
- When does the next bus leave from *this* stop on *this* route?

The experiment was to see how far one can get with an **in-memory read model**:
load the whole city into RAM once at start-up, cross-link routes to stops to
timetables, index it for fast lookup, and serve the passenger's questions over
a plain REST API — no database, no ORM, no caching layer.

For a city-sized dataset this is entirely reasonable: a few thousand stops and
a few hundred routes fit comfortably in memory, and every query becomes a
dictionary lookup or a single pass over a list.

## What the experiment covers

- **Import** — parsing the operator's XML and JSON dumps into a domain model
  ([Data Model](03-DataModel.md), [Data Sources](04-DataSources.md)).
- **Cross-linking** — stitching routes, stops and schedules into one graph, and
  discarding whatever cannot be linked ([Architecture](02-Architecture.md)).
- **Indexing** — a sorted index of stops by their public stop code, so that
  "stop 3899" is an O(log n) lookup rather than a scan ([Geo and Search](06-Geo-Search.md)).
- **Geo search** — finding the stops within *N* kilometres of a coordinate,
  using a great-circle distance over the Earth's surface ([Geo and Search](06-Geo-Search.md)).
- **API** — a versioned ASP.NET Core Web API with Swagger ([API](05-API.md)).
- **A throwaway UI** — a Razor Pages site used only to prove the API is
  reachable from a browser across CORS.

## What it deliberately is not

- Not a journey planner. There is no routing between two points, no transfers,
  no "how do I get from A to B".
- Not real-time. The data is a static snapshot of the published timetable;
  there is no vehicle tracking or arrival prediction.
- Not multi-city. Requests carry a `CityId`, but the service loads exactly one
  dataset and ignores the parameter.
- Not multi-lingual yet. Requests carry a `Language`, and the import tags
  everything as `ANY`; no translation data was ever wired in.

Those parameters are placeholders left in the API shape on purpose: the
experiment was about the data pipeline, and the request contracts were designed
so that a city dimension and a language dimension could be added later without
breaking clients.

## Data used

The dataset is Tbilisi's municipal bus network. Route long names are in
Georgian, stop names are transliterated into Latin script, and coordinates sit
around 41.8 N / 44.8 E.

The live source the data was originally pulled from is no longer reachable, but
a working snapshot of five routes survives inside the test project and is enough
to run every part of the pipeline. See [Data Sources](04-DataSources.md).

## Status

The pipeline works end to end: the service starts, imports the snapshot, and
answers all five endpoints. It is an experiment, not a product — see
[Status and Backlog](08-Status-and-Backlog.md) for what is done, what is
half-done, and the defects found while writing this documentation.
