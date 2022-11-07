using BusTable.Core.Dto;
using BusTable.Core.Import;
using BusTable.Core.Models;

namespace BusTable.Service.Services
{
    public class ImportService
    {
        private readonly StopService _stopDataService;

        public ImportService(StopService stopService)
        {
            _stopDataService = stopService;
        }

        public BusRouteData LoadRouteData(string fileName)
        {
            RouteList routeList = RouteList.Load(fileName);

            BusRouteData data = new()
            {
                Language = "ANY"
            };

            foreach (var routeIn in routeList.Routes.Values.ToList())
            {
                if (routeIn.RouteNumber == 0)
                {
                    continue;
                }

                if (data.Items.ContainsKey(routeIn.RouteNumber))
                {
                    continue;
                }

                data.Items.Add(routeIn.RouteNumber, new()
                {
                    Id = routeIn.RouteNumber,
                    Name = routeIn.LongName,
                    Point1 = routeIn.StopA,
                    Point2 = routeIn.StopB,
                });
            }

            return data;
        }

        public Dictionary<int, StopData> LoadPointData(IEnumerable<int> routeIds, string directory)
        {
            var ix = routeIds.Distinct().ToHashSet();
            var points = new Dictionary<int, StopData>();
            var fileNames = Directory.EnumerateFiles(directory, "*f1.xml");

            foreach (var fileName in fileNames)
            {
                StopData data = LoadPointData(fileName);

                if (ix.Contains(data.RouteId))
                {
                    points[data.RouteId] = data;
                }
            }

            return points;
        }

        public StopData LoadPointData(string fileName)
        {
            RouteSchedule schedule = RouteSchedule.Load(fileName);

            StopData data = new()
            {
                Language = "ANY",
                RouteId = schedule.RouteNumber,
            };

            foreach (RouteStop input in schedule.RouteStops.Values.ToList())
            {
                if (input.StopId == 0)
                {
                    throw new Exception($"The {nameof(RouteStop)} has not {nameof(input.StopId)} value: {input}");
                }
                if (!_stopDataService.TryGetById(input.StopId, out StopInfo? stopItem))
                {
                    _stopDataService.AddStop(input);

                    if (!_stopDataService.TryGetById(input.StopId, out stopItem))
                    {
                        throw new Exception($"The {nameof(StopService)} does not contain stop[{input.StopId}] value for {input}");
                    }
                }

                if (stopItem == null)
                {
                    continue;
                }

                data.Items.Add(new()
                {
                    Id = stopItem.Id,
                    Lon = stopItem.Lon,
                    Lat = stopItem.Lat,
                    Name = stopItem.Name,
                });
            }
            return data;
        }
    }
}
