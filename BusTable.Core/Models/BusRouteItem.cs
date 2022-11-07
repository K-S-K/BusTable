namespace BusTable.Core.Models
{
    public class BusRouteItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Point1 { get; set; } = null!;
        public string Point2 { get; set; } = null!;
        public bool Circle => Point1 == Point2;

        public override string ToString()
        {
            return $"[{Id}]: \"{Point1}\" - \"{Point2}\", \"{Name}\"";
        }
    }
}
