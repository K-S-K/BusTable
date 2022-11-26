using BusTable.Core.Dto;

namespace BusTable.Service.Services
{
    public class RouteService
    {
        private readonly LanguageValidationService _languageValidationService;
        private readonly StopService _stopDataService;
        private readonly ImportService _importService;

        private readonly Dictionary<string, StopData> stopData = new();
        private BusRouteData routeData;

        public StopData? GetRouteStops(string language, string routeNumber, int cityId = 0)
        {
            return stopData.TryGetValue(routeNumber, out var data) ? data : null;
        }

        public BusRouteData GetRoutes(string language, int cityId = 0)
        {
            return routeData;
        }

        public RouteService(StopService stopDataService,
            LanguageValidationService languageValidationService,
            ImportService importService
            )
        {
            _languageValidationService = languageValidationService;
            _stopDataService = stopDataService;
            _importService = importService;

            routeData = _importService.LoadRouteData(@"C:\Polygon\BusTable\SourceData\routes.xml");
            stopData = _importService.LoadStopData(routeData.Items.Keys, @"C:\Polygon\BusTable\SourceData\", _stopDataService);

            HashSet<string> routeIds = stopData.Keys.ToHashSet();
            HashSet<string> routeDel = new();

            foreach (var id in routeData.Items.Keys)
            {
                if (!routeIds.Contains(id))
                {
                    routeDel.Add(id);
                }
            }

            routeDel.ToList().ForEach(x => routeData.Items.Remove(x));
        }
    }
}
