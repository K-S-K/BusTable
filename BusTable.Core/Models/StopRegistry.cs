using System.Text.Json;

namespace BusTable.Core.Models
{
    public class StopRegistry
    {
        private readonly SortedDictionary<int, StopInfo> _ixCode = new();

        public List<StopInfo> Stops { get; set; } = new();

        public bool TryGetById(int code, out StopInfo? item) => _ixCode.TryGetValue(code, out item);

        public void Load(string fileName)
        {
            Stops.Clear();
            _ixCode.Clear();

            var input = File.ReadAllText(fileName);
            var data = JsonSerializer.Deserialize<List<Import.StopItem>>(input);
            if (data == null)
            {
                return;
            }

            foreach (var item in data)
            {
                if (item.Code == 0)
                {
                    continue;
                }

                StopInfo stopInfo = new()
                {
                    Id = item.Code,
                    Lat = item.Lat,
                    Lon = item.Lon,
                    Name = item.Name,
                };

                _ixCode.Add(stopInfo.Id, stopInfo);
            }

            Stops = _ixCode.Values.ToList();
        }

        public void AddStop(Import.RouteStop item)
        {
            if (_ixCode.ContainsKey(item.StopId))
            {
                throw new Exception($"{nameof(StopRegistry)} already contains stop [{item.StopId}]: can't merge {item} to {_ixCode[item.StopId]}");
            }

            StopInfo stopInfo = new()
            {
                Id = item.StopId,
                Lon = item.Lon,
                Lat = item.Lat,
                Name = item.Name ?? string.Empty,
            };

            _ixCode.Add(stopInfo.Id, stopInfo);
            Stops.Add(stopInfo);
        }

        public override string ToString() => $"Count: {Stops.Count}";
    }
}
