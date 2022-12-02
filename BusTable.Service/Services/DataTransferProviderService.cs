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

            StopData? stops = await _routeService.GetRouteStops(request);
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

        public async Task<StopData?> GetRouteStops(BusRouteStopsRequest request)
        {
            try
            {
                _languageValidationService.Validate(request.Language);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }

            StopData? data = await _routeService.GetRouteStops(request);

            return data;
        }

        public async Task<BusRouteData> GetRoutes(BusRoutesRequest request)
        {
            try
            {
                _languageValidationService.Validate(request.Language);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }

            return await _routeService.GetRoutes(request);
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
