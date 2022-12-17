using BusTable.Core.Dto;
using BusTable.Core.Import;
using BusTable.Core.Models;

namespace BusTable.Service.Services
{
    public class RouteService
    {
        private readonly IImportService _importService;

        private readonly ScheduleRegistry scheduleRegistry;
        private readonly RouteRegistry routeRegistry;
        private readonly StopRegistry stopRegistry;

        public async Task<BusDepartureTimeData?> GetBusDepartureTimesForTheStop(BusDepartureTimesForTheStopRequest request)
        {
            StopRouteSchedule? stops = await GetRouteStops(request);
            if (stops == null)
            {
                return null;
            }

            StopInfo? si = stops.Items.Where(x => x.Id == request.StopID).FirstOrDefault();
            if (si == null)
            {
                return null;
            }

            BusDepartureTimeData data = new()
            {
                Language = stops.Language,
                StopId = request.StopID,
                StopName = si.Name,
                Times = si.ArriveTimes,
            };

            return data;
        }

        public async Task<StopRouteSchedule?> GetRouteStops(IBusRouteStopsRequest request)
        {
            var result = await scheduleRegistry.GetRouteStops(request);

            return result;
        }

        public async Task<BusRouteData> GetRoutes(BusRoutesRequest request)
        {
            IQueryable<BusRouteItem> items = await routeRegistry.GetRoutes(request);

            BusRouteData result = new()
            {
                Language = request.Language,
                PageNumber = request.PageNumber
            };
            foreach (var item in items)
            {
                result.Items.Add(item);
            }

            return result;
        }


        public bool TryGetStopById(int code, out StopHeader? item) => stopRegistry.TryGetById(code, out item);

        public IEnumerable<StopHeader> GetStops(string language, double lat, double lon)
        {
            return stopRegistry.Stops;
        }

        public void AddStop(RouteStop item) => stopRegistry.AddStop(item);

        public RouteService(IImportService importService)
        {
            _importService = importService;

            stopRegistry = _importService.LoadStopRegistry();
            routeRegistry = _importService.LoadRouteRegistry();
            scheduleRegistry = _importService.LoadScheduleRegistry(routeRegistry.Items.Keys, this);

            HashSet<string> routeIds = scheduleRegistry.stopData.Keys.ToHashSet();
            HashSet<string> routeDel = new();

            foreach (var id in routeRegistry.Items.Keys)
            {
                if (!routeIds.Contains(id))
                {
                    routeDel.Add(id);
                }
            }

            routeDel.ToList().ForEach(x => routeRegistry.Items.Remove(x));
        }
    }
}
