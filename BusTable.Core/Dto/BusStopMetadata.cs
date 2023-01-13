namespace BusTable.Core.Dto
{
    public class BusStopMetadata
    {
        public int Count => Items.Count;
        public int PageNumber { get; set; } = 1;
        public string Language { get; set; } = null!;
        public List<BusStopHeader> Items { get; set; } = new();

        public override string ToString() => $"{nameof(Count)}:{Count} {nameof(Language)}:{Language}";
    }
}
