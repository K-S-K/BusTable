using BusTable.Core.Dto;
using BusTable.Core.Models;

namespace BusTable.Service.Services
{
    public interface IImportService
    {
        RouteRegistry LoadRouteRegistry();
        StopRouteSchedule LoadRouteSchedule(string fileName, RouteService routeService);
        Dictionary<string, StopRouteSchedule> LoadStopData(IEnumerable<string> routeIds, RouteService routeService);
        StopRegistry LoadStopRegistry();
    }
}