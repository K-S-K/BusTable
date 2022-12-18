using System.Text.Json;

using BusTable.Core.Dto;

namespace BusTable.Core.Models
{
    public class StopRegistry
    {
        private readonly SortedDictionary<int, StopHeader> _ixCode = new();

        public List<StopHeader> Stops { get; set; } = new();

        public bool TryGetById(int code, out StopHeader? item) => _ixCode.TryGetValue(code, out item);

        public async Task<IEnumerable<BusStopHeader>> GetStops(BusStopsRequest request)
        {
            IQueryable<BusStopHeader> items = (await Task.FromResult(Stops.Select(x =>
            new BusStopHeader()
            {
                Id = x.Id,
                Name = x.Name,
                Lat = x.Lat,
                Lon = x.Lon,
                Distance = x.GetDistance(request.Lat, request.Lon)
            }
            ))).AsQueryable();

            if (request.DistanceCencitive)
            {
                items = items
                    .Where(x => (x.Distance <= request.MaxDistance));
            }

            return items;
        }

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

                StopHeader stopHeader = new()
                {
                    Id = item.Code,
                    Lat = item.Lat,
                    Lon = item.Lon,
                    Name = item.Name,
                };

                _ixCode.Add(stopHeader.Id, stopHeader);
            }

            Stops = _ixCode.Values.ToList();
        }

        public void AddStop(Import.RouteStop item)
        {
            if (_ixCode.ContainsKey(item.StopId))
            {
                throw new Exception($"{nameof(StopRegistry)} already contains stop [{item.StopId}]: can't merge {item} to {_ixCode[item.StopId]}");
            }

            StopHeader stopHeader = new()
            {
                Id = item.StopId,
                Lon = item.Lon,
                Lat = item.Lat,
                Name = item.Name ?? string.Empty,
            };

            _ixCode.Add(stopHeader.Id, stopHeader);
            Stops.Add(stopHeader);
        }

        public override string ToString() => $"Count: {Stops.Count}";
    }
}
