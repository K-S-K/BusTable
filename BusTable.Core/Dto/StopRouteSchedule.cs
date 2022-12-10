using BusTable.Core.Models;

namespace BusTable.Core.Dto
{
    public class StopRouteSchedule
    {
        public string Language { get; set; } = null!;
        public string RouteNumber { get; set; } = null!;
        public List<StopInfo> Items { get; set; } = new();

        public override string ToString() => $"[{RouteNumber}] StopCount:{Items.Count}";
    }
}
