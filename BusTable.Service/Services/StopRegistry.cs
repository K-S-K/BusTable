using BusTable.Core.Import;
using BusTable.Core.Models;

namespace BusTable.Service.Services
{
    public class StopService
    {
        private readonly StopRegistry _stopRegistry;


        public StopService()
        {
            _stopRegistry = new();

            var fileName = @"C:\Polygon\BusTable\SourceData\stops-k.json";
            _stopRegistry.Load(fileName);
        }

        public bool TryGetById(int code, out StopInfo? item) => _stopRegistry.TryGetById(code, out item);

        public IEnumerable<StopInfo> GetStops(string language, double lat, double lon)
        {
            return _stopRegistry.Stops;
        }

        public void AddStop(RouteStop item) => _stopRegistry.AddStop(item);
    }
}
