using System.Xml.Linq;

namespace BusTable.Core.Import
{
    public class RouteList
    {
        public Dictionary<string, RouteItem> Routes { get; set; } = new();


        public XElement XContent
        {
            get
            {
                return new XElement(nameof(RouteList),
                    new XAttribute(nameof(Routes.Count), Routes.Count),
                    Routes.Values.ToList().Select(x => x.XContent)
                    );
            }
            set
            {
                var rawRoutes = value?.Elements("Route").Select(x => new RouteItem(x));

                if (rawRoutes != null)
                {
                    foreach (var route in rawRoutes)
                    {
                        if (route.RouteNumber != "0")
                        {
                            Routes.Add(route.RouteNumber, route);
                        }
                    }
                }
            }
        }

        public static RouteList Load(string fileName)
        {
            XElement? xe = XDocument.Load(fileName)?.Root;
            if (xe == null)
            {
                throw new Exception($"Can't load file {fileName}");
            }

            return new RouteList(xe);
        }

        public override string ToString() => $"Count:{Routes.Count}";

        public RouteList() { }
        public RouteList(XElement xe) : this() { this.XContent = xe; }
    }
}
