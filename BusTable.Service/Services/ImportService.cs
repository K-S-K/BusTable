using BusTable.Core.Dto;
using BusTable.Core.Import;
using BusTable.Core.Models;
using BusTable.Service.Settings;

namespace BusTable.Service.Services
{
    public class ImportService : IImportService
    {
        private readonly ImportSourceSettings? _settings;
        private ImportSourceSettings Settings
        {
            get
            {
                return _settings ??
                    throw new NotImplementedException(
                    $"{nameof(_settings)} was not provided");
            }
        }

        public ImportService(ImportSourceSettings settings)
        {
            _settings = settings;
        }

        public RouteRegistry LoadRouteRegistry()
        {
            string fileName = Path.Combine(Settings.Directory, Settings.RouteListFileName);

            RouteList routeList = RouteList.Load(fileName);

            RouteRegistry data = new()
            {
                Language = "ANY"
            };

            routeList.Routes.Values.ToList().ForEach(x => data.Add(x));

            return data;
        }

        public ScheduleRegistry LoadScheduleRegistry(IEnumerable<string> routeIds, RouteService routeService)
        {
            ScheduleRegistry data = new()
            {
                stopData = LoadStopData(routeIds, routeService)
            };

            return data;
        }

        public StopRouteSchedule ApplyRouteSchedule(RouteService routeService, RouteSchedule schedule)
        {
            StopRouteSchedule data = new()
            {
                Language = "ANY",
                RouteNumber = schedule.RouteNumber,
            };

            foreach (RouteStop input in schedule.RouteStops.Values.ToList())
            {
                if (input.StopId == 0)
                {
                    throw new Exception($"The {nameof(RouteStop)} has not {nameof(input.StopId)} value: {input}");
                }
                if (!routeService.TryGetStopById(input.StopId, out StopHeader? stopHeader))
                {
                    routeService.AddStop(input);
                }

                if (stopHeader == null)
                {
                    continue;
                }

                data.Items.Add(new()
                {
                    Id = stopHeader.Id,
                    Lon = stopHeader.Lon,
                    Lat = stopHeader.Lat,
                    Name = stopHeader.Name,
                    ArriveTimes = input.ArriveTimes.Select(x => new BusDepartureTimeItem() { Time = x }).ToList(),
                });
            }
            return data;
        }

        protected Dictionary<string, StopRouteSchedule> LoadStopData(IEnumerable<string> routeIds, RouteService routeService)
        {
            var ix = routeIds.Distinct().ToHashSet();
            var stops = new Dictionary<string, StopRouteSchedule>();
            var fileNames = Directory.EnumerateFiles(Settings.Directory, "*f1.xml");

            foreach (var fileName in fileNames)
            {
                StopRouteSchedule data = LoadRouteSchedule(fileName, routeService);

                if (ix.Contains(data.RouteNumber))
                {
                    stops[data.RouteNumber] = data;
                }
            }

            return stops;
        }

        private StopRouteSchedule LoadRouteSchedule(string fileName, RouteService routeService)
        {
            RouteSchedule schedule = RouteSchedule.Load(fileName);

            return ApplyRouteSchedule(routeService, schedule);
        }

        public StopRegistry LoadStopRegistry()
        {
            string fileName = Path.Combine(Settings.Directory, Settings.StopListFileName);

            StopRegistry data = new();
            data.Load(fileName);
            return data;
        }
    }
}
