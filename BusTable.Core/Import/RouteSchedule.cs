using System.Xml.Linq;

namespace BusTable.Core.Import
{
    public class RouteSchedule
    {
        public string RouteNumber { get; set; } = null!;
        public bool Forward { get; set; }
        public Dictionary<int, RouteStop> RouteStops { get; set; } = new();

        public XElement XContent
        {
            get
            {
                return new XElement(nameof(RouteSchedule),
                    new XElement(nameof(RouteNumber), RouteNumber),
                    new XElement(nameof(Forward), Forward),
                    new XElement(nameof(RouteStops), RouteStops.Values.ToList().Select(x => x.XContent))
                    );
            }
            set
            {
                RouteNumber = value?.Element(nameof(RouteNumber))?.Value ?? "0";
                Forward = bool.Parse(value?.Element(nameof(Forward))?.Value ?? false.ToString());
                var rawStops = value?.Element("WeekdaySchedules")
                    ?.Elements("Stops").Select(x => new RouteStop(x));

                if (rawStops != null)
                {
                    foreach (var stop in rawStops)
                    {
                        if (stop.StopId != 0)
                        {
                            RouteStops.Add(stop.StopId, stop);
                        }
                    }
                }
            }
        }

        public static RouteSchedule Load(string fileName)
        {
            XElement? xe = XDocument.Load(fileName)?.Root;
            if (xe == null)
            {
                throw new Exception($"Can't load file {fileName}");
            }

            return new RouteSchedule(xe);
        }

        public override string ToString()
        {
            return $"[{RouteNumber}] {(Forward ? "=>" : "<=")} StopCount:{RouteStops.Count}";
        }


        public RouteSchedule(XElement xe)
        {
            XContent = xe;
        }
    }
}
