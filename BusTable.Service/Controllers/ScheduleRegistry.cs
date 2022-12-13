using BusTable.Core.Dto;

namespace BusTable.Service.Services
{
    public class ScheduleRegistry
    {
        public Dictionary<string, StopRouteSchedule> stopData = new();

        public async Task<StopRouteSchedule?> GetRouteStops(IBusRouteStopsRequest request)
        {
            var result = stopData.TryGetValue(request.RouteNumber, out var data) ? data : null;

            return (await Task.FromResult(result));
        }
    }
}
