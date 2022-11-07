using BusTable.Core.Dto;

namespace BusTable.Service.Services
{
    public class RouteService
    {
        private readonly LanguageValidationService _languageValidationService;
        private readonly StopService _stopDataService;
        private readonly ImportService _importService;

        private readonly Dictionary<int, StopData> pointData = new();
        private BusRouteData routeData;

        public StopData? GetRoutePoints(string language, int routeId, int cityId = 0)
        {
            return pointData.TryGetValue(routeId, out var data) ? data : null;
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
            pointData = _importService.LoadPointData(routeData.Items.Keys, @"C:\Polygon\BusTable\SourceData\");

            HashSet<int> routeIds = pointData.Keys.ToHashSet();
            HashSet<int> routeDel = new();

            foreach (int id in routeData.Items.Keys)
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
