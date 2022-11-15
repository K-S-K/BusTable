using System.Text.Json.Serialization;

namespace BusTable.Core.Models
{
    public class BusDepartureTimeItem
    {
        [JsonIgnore]
        public TimeSpan Time { get; set; }

        public string Departure => Time.ToString("hh\\:mm");
    }
}
