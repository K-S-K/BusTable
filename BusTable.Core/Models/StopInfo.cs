namespace BusTable.Core.Models
{
    public class StopInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public double Lon { get; set; }
        public double Lat { get; set; }

        public override string ToString() => $"[{Id}] \"{Name}\"";
    }
}
