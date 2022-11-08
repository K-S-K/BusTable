using BusTable.Core.Dto;
using BusTable.Core.Common;

namespace BusTable.Service.Services
{
    public class DataTransferProviderService
    {
        private readonly LanguageValidationService _languageValidationService;
        private readonly StopService _stopDataService;
        private readonly RouteService _routeService;

        public BusDepartureTimeData GetBusDepartureTimesForThLine(string language, int lineId)
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
            data.Times.Add(new() { Departure = new TimeSpan(8, 32, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(8, 46, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(8, 58, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(9, 10, 0) });

            return data;
        }

        public BusDepartureTimeData GetBusDepartureTimesForTheStop(string language, int stopId)
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
            BusDepartureTimeData data = new()
            {
                Language = language,
                StopId = stopId, // 20237
                StopName = "Opposite to Zakaria Paliashvili Street #33a"
                // Напротив улицы Закария Палиашвили №33а 
            };
            data.Times.Add(new() { Departure = new TimeSpan(7, 14, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(7, 34, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(7, 54, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(8, 10, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(8, 22, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(8, 34, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(8, 46, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(8, 58, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(9, 10, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(9, 22, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(9, 34, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(9, 46, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(9, 58, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(10, 11, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(10, 26, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(10, 41, 0) });
            data.Times.Add(new() { Departure = new TimeSpan(10, 56, 0) });

            return data;
        }

        public StopData? GetRoutePoints(string language, int routeId, int cityId = 0)
        {
            try
            {
                _languageValidationService.Validate(language);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }

            StopData? data = _routeService.GetRoutePoints(language, routeId, cityId);

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
