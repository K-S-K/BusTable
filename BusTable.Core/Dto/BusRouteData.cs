using BusTable.Core.Models;

namespace BusTable.Core.Dto
{
    public class BusRouteData
    {
        public string Language { get; set; } = null!;
        public Dictionary<int, BusRouteItem> Items { get; set; } = new();

        public override string ToString() => $"{nameof(Items.Count)}:{Items.Count} {nameof(Language)}:{Language}";
    }
}
