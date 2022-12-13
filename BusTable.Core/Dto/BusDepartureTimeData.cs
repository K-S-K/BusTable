using BusTable.Core.Models;

namespace BusTable.Core.Dto
{
    public class BusDepartureTimeData
    {
        public int StopId { get; set; }
        public string StopName { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int Count => Times.Count;
        public List<BusDepartureTimeItem> Times { get; set; } = new();

        public override string ToString() => $"[{StopId}] \"{StopName}\"";
    }
}
