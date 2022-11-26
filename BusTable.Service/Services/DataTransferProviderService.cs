using BusTable.Core.Dto;
using BusTable.Core.Common;
using BusTable.Core.Models;

namespace BusTable.Service.Services
{
    public class DataTransferProviderService
    {
        private readonly LanguageValidationService _languageValidationService;
        private readonly StopService _stopDataService;
        private readonly RouteService _routeService;

        public BusDepartureTimeData GetBusDepartureTimesForTheRoute(string language, string routeNumber)
        {

            try
            {
                _languageValidationService.Validate(language);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }

            // TODO: It must be from the Data Layer
            // TODO: It must be from the Depency Injection

            BusDepartureTimeData data = new();
            data.StopName = "Abashidze";
            data.Times.Add(new() { Time = new TimeSpan(8, 32, 0) });
            data.Times.Add(new() { Time = new TimeSpan(8, 46, 0) });
            data.Times.Add(new() { Time = new TimeSpan(8, 58, 0) });
            data.Times.Add(new() { Time = new TimeSpan(9, 10, 0) });

            return data;
        }

        public BusDepartureTimeData? GetBusDepartureTimesForTheStop(string language, string routeNumber, int stopId)
        {
            try
            {
                _languageValidationService.Validate(language);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }

            StopData? stops = _routeService.GetRouteStops(language, routeNumber);
            if (stops == null)
            {
                return null;
            }

            StopInfo? si = stops.Items.Where(x => x.Id == stopId).FirstOrDefault();
            if (si == null)
            {
                return null;
            }

            BusDepartureTimeData data = new()
            {
                Language = stops.Language,
                StopId = stopId,
                StopName = si.Name,
                Times = si.ArriveTimes,
            };

            return data;
        }

        public StopData? GetRouteStops(string language, string routeId, int cityId = 0)
        {
            try
            {
                _languageValidationService.Validate(language);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }

            StopData? data = _routeService.GetRouteStops(language, routeId, cityId);

            return data;
        }

        public BusRouteData GetRoutes(string language, int cityId = 0)
        {
            try
            {
                _languageValidationService.Validate(language);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }

            return _routeService.GetRoutes(language, cityId);
        }

        public DataTransferProviderService(
            LanguageValidationService languageValidationService,
            StopService stopDataService,
            RouteService routeService)
        {
            _languageValidationService = languageValidationService;
            _stopDataService = stopDataService;
            _routeService = routeService;
        }
    }
}
