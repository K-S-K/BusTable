using BusTable.Core.Dto;
using BusTable.Core.Models;

namespace BusTable.Service.Services
{
    public interface IImportService
    {
        RouteRegistry LoadRouteRegistry();
        StopData LoadRouteSchedule(string fileName, StopService stopDataService);
        Dictionary<string, StopData> LoadStopData(IEnumerable<string> routeIds, StopService stopDataService);
        StopRegistry LoadStopRegistry();
    }
}