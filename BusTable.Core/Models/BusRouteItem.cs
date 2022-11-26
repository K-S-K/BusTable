namespace BusTable.Core.Models
{
    public class BusRouteItem
    {
        public string Number { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Stop1 { get; set; } = null!;
        public string Stop2 { get; set; } = null!;
        public bool Circle => Stop1 == Stop2;

        public override string ToString()
        {
            return $"[{nameof(Number)}]: \"{Stop1}\" - \"{Stop2}\", \"{Name}\"";
        }
    }
}
