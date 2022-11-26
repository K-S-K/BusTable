using BusTable.Core.Import;
using BusTable.Core.Models;

namespace BusTable.Service.Services
{
    public class StopService
    {
        private readonly ImportService _importService;
        private readonly StopRegistry _stopRegistry;

        public StopService(ImportService importService)
        {
            _importService = importService;

            _stopRegistry = _importService.LoadStopRegistry();
        }

        public bool TryGetById(int code, out StopHeader? item) => _stopRegistry.TryGetById(code, out item);

        public IEnumerable<StopHeader> GetStops(string language, double lat, double lon)
        {
            return _stopRegistry.Stops;
        }

        public void AddStop(RouteStop item) => _stopRegistry.AddStop(item);
    }
}
