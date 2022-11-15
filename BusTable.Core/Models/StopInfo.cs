using System.Text.Json.Serialization;

namespace BusTable.Core.Models
{
    public class StopInfo : StopHeader
    {
        [JsonIgnore]
        public List<BusDepartureTimeItem> ArriveTimes { get; set; } = new();
    }
}
