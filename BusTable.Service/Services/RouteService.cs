using BusTable.Core.Common;
using BusTable.Core.Dto;
using BusTable.Core.Models;

namespace BusTable.Service.Services
{
    public class RouteService
    {
        private readonly LanguageValidationService _languageValidationService;
        private readonly StopService _stopDataService;
        private readonly IImportService _importService;

        private readonly Dictionary<string, StopRouteSchedule> stopData = new();
        private readonly RouteRegistry routeRegistry;

        public async Task<BusDepartureTimeData?> GetBusDepartureTimesForTheStop(BusDepartureTimesForTheStopRequest request)
        {
            try
            {
                _languageValidationService.Validate(request.Language);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }

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
            var result = stopData.TryGetValue(request.RouteNumber, out var data) ? data : null;

            return (await Task.FromResult(result));
        }

        public async Task<BusRouteData> GetRoutes(BusRoutesRequest request)
        {
            return await routeRegistry.GetRoutes(request);
        }

        public RouteService(StopService stopDataService,
            LanguageValidationService languageValidationService,
            IImportService importService
            )
        {
            _languageValidationService = languageValidationService;
            _stopDataService = stopDataService;
            _importService = importService;

            routeRegistry = _importService.LoadRouteRegistry();
            stopData = _importService.LoadStopData(routeRegistry.Items.Keys, _stopDataService);

            HashSet<string> routeIds = stopData.Keys.ToHashSet();
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
