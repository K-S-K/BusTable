using System.Text.Json.Serialization;

namespace BusTable.Core.Dto
{
    public class BusStopsRequest : RequestWithPagination
    {
        public double Lon { get; set; } = 44.82726800000056;
        public double Lat { get; set; } = 41.80539999998982;
        public double MaxDistance { get; set; } = 5.0;

        [JsonIgnore]
        public bool DistanceCencitive => Lat > 1.0 && Lon > 1.0 && MaxDistance > 0.1;
    }
}
