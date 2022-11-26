namespace BusTable.Core.Models
{
    public class BusRouteItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Stop1 { get; set; } = null!;
        public string Stop2 { get; set; } = null!;
        public bool Circle => Stop1 == Stop2;

        public override string ToString()
        {
            return $"[{Id}]: \"{Stop1}\" - \"{Stop2}\", \"{Name}\"";
        }
    }
}
