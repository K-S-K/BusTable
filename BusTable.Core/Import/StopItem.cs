using System.Text;
using System.Text.Json.Serialization;

namespace BusTable.Core.Import
{
    public class StopItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("code")]
        public string? CodeString { get; set; }

        [JsonIgnore]
        public int Code => int.Parse(CodeString ?? "0");

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new();

            sb.Append($"[{Code}]");
            // sb.Append($" Lat:{Lat:00.####}, Lon:{Lon:00:####}");
            sb.Append($" \"{Name}\"");

            return sb.ToString();
        }
    }
}
