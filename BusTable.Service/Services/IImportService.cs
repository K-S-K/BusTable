using BusTable.Core.Dto;
using BusTable.Core.Models;

namespace BusTable.Service.Services
{
    public interface IImportService
    {
        RouteRegistry LoadRouteRegistry();
        StopRouteSchedule LoadRouteSchedule(string fileName, StopService stopDataService);
        Dictionary<string, StopRouteSchedule> LoadStopData(IEnumerable<string> routeIds, StopService stopDataService);
        StopRegistry LoadStopRegistry();
    }
}