using BusTable.Core.Dto;
using BusTable.Core.Models;

namespace BusTable.Service.Services
{
    public class RouteService
    {
        private readonly LanguageValidationService _languageValidationService;
        private readonly StopService _stopDataService;
        private readonly ImportService _importService;

        private readonly Dictionary<string, StopData> stopData = new();
        private readonly RouteRegistry routeRegistry;

        public StopData? GetRouteStops(string language, string routeNumber, int cityId = 0)
        {
            return stopData.TryGetValue(routeNumber, out var data) ? data : null;
        }

        public async Task<BusRouteData> GetRoutes(BusRoutesRequest request)
        {
            return await routeRegistry.GetRoutes(request);
        }

        public RouteService(StopService stopDataService,
            LanguageValidationService languageValidationService,
            ImportService importService
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
