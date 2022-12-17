using BusTable.Core.Dto;
using BusTable.Core.Models;

namespace BusTable.Service.Services
{
    public interface IImportService
    {
        RouteRegistry LoadRouteRegistry();
        ScheduleRegistry LoadScheduleRegistry(IEnumerable<string> routeIds, RouteService routeService);
        StopRegistry LoadStopRegistry();
    }
}