using System.Text;
using System.Xml.Linq;
using System.Globalization;

namespace BusTable.Core.Import
{
    public class RouteStop
    {
        private const string fmt = "h\\:mm";
        private CultureInfo clt = CultureInfo.InvariantCulture;
        private TimeSpanStyles stl = TimeSpanStyles.None;

        public RouteStop(XElement xe)
        {
            XContent = xe;
        }

        public List<TimeSpan> ArriveTimes { get; set; } = new();
        public double Lat { get; set; }
        public double Lon { get; set; }
        public int Sequence { get; set; }
        public int StopId { get; set; }
        public string? Name { get; set; }

        public XElement XContent
        {
            get
            {
                StringBuilder sb = new();
                ArriveTimes.ForEach(x => sb.Append($"{(sb.Length == 0 ? string.Empty : ",")}{x.ToString(fmt)}"));
                return new XElement(nameof(RouteStop),
                    new XElement(nameof(ArriveTimes), sb.ToString()),
                    new XElement(nameof(Lat), Lat),
                    new XElement(nameof(Lon), Lon),
                    new XElement(nameof(Sequence), Sequence),
                    new XElement(nameof(StopId), StopId),
                    new XElement(nameof(Name), Name));
            }
            set
            {
                var arriveTimesLine = value.Element(nameof(ArriveTimes))?.Value ?? string.Empty;
                var arriveTimesStrings = arriveTimesLine.Split(',');
                ArriveTimes = arriveTimesStrings.ToList()
                    .Select(x => x.Replace("24:", "0:"))
                    .Select(x => x.Replace("25:", "1:"))
                    .Select(x => x.Replace("26:", "2:"))
                    .Select(x => TimeSpan.ParseExact(x, fmt, clt, stl))
                    .ToList().OrderBy(x => x).ToList();
                Lat = double.Parse(value?.Element(nameof(Lat))?.Value ?? "0.0");
                Lon = double.Parse(value?.Element(nameof(Lon))?.Value ?? "0.0");
                Sequence = int.Parse(value?.Element(nameof(Sequence))?.Value ?? "0");
                string stopIdStr = value?.Element(nameof(StopId))?.Value ?? "0";

                try { StopId = int.Parse(stopIdStr); }
                catch (Exception ex)
                {
                    throw new Exception($"Cannot parse value \"{stopIdStr}\" as int: {ex.Message}");
                }

                string name = value?.Element(nameof(Name))?.Value ?? string.Empty;
                Name = name.Replace($" - [{StopId}]", string.Empty).Trim('\"');
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            sb.Append($"[{StopId}]");
            sb.Append($" Lat:{Lat:00.#####}, Lon:{Lon:00.#####}");

            return sb.ToString();
        }
    }
}
