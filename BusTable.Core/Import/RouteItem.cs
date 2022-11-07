using System.Xml.Linq;

namespace BusTable.Core.Import
{
    public class RouteItem
    {
        public int RouteNumber { get; set; }
        public string LongName { get; set; } = null!;
        public string StopA { get; set; } = null!;
        public string StopB { get; set; } = null!;

        public RouteItem() { }
        public RouteItem(XElement xe) : this() { this.XContent = xe; }

        public XElement XContent
        {
            get
            {
                return new XElement("Route",
                    new XElement(nameof(RouteNumber), RouteNumber),
                    new XElement(nameof(LongName), LongName),
                    new XElement(nameof(StopA), StopA),
                    new XElement(nameof(StopB), StopB));
            }
            set
            {
                LongName = value.Element(nameof(LongName))?.Value ?? string.Empty;
                StopA = value.Element(nameof(StopA))?.Value ?? string.Empty;
                StopB = value.Element(nameof(StopB))?.Value ?? string.Empty;
                string routeNumberStr = value?.Element(nameof(RouteNumber))?.Value ?? "0";

                try { RouteNumber = int.Parse(routeNumberStr); }
                catch (Exception ex)
                {
                    throw new Exception($"Cannot parse value \"{routeNumberStr}\" as int: {ex.Message}");
                }
            }
        }

        public override string ToString()
        {
            return $"[{RouteNumber}]: {LongName}";
        }
    }
}
