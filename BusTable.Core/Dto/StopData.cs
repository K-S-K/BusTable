using BusTable.Core.Models;

namespace BusTable.Core.Dto
{
    public class StopData
    {
        public string Language { get; set; } = null!;
        public int RouteId { get; set; }
        public List<StopInfo> Items { get; set; } = new();

        public override string ToString() => $"[{RouteId}] StopCount:{Items.Count}";
    }
}
