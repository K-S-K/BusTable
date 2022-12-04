using BusTable.Core.Models;

namespace BusTable.Core.Dto
{
    public class BusRouteData
    {
        public int Count => Items.Count;
        public int PageNumber { get; set; } = 1;
        public string Language { get; set; } = null!;
        public List<BusRouteItem> Items { get; set; } = new();

        public override string ToString() => $"{nameof(Count)}:{Count} {nameof(Language)}:{Language}";
    }
}
