using BusTable.Core.Dto;
using BusTable.Core.Import;

namespace BusTable.Core.Models
{
    public class RouteRegistry
    {
        public string Language { get; set; } = null!;
        public Dictionary<string, BusRouteItem> Items { get; set; } = new();

        public async Task<BusRouteData> GetRoutes(BusRoutesRequest request)
        {
            IQueryable<BusRouteItem> items = (await Task.FromResult(Items.Values.ToList())).AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                items = items
                    .Where(x =>
                                x.Name.ToLower().Contains(search)
                                ||
                                x.Stop1.ToLower().Contains(search)
                                ||
                                x.Stop2.ToLower().Contains(search)
                    );
            }

            items = items
            .Skip(request.PageSize * (request.PageNumber - 1))
            .Take(request.PageSize);

            BusRouteData result = new()
            {
                Language = Language,
                PageNumber = request.PageNumber
            };
            foreach (var item in items)
            {
                result.Items.Add(item);
            }

            return result;
        }

        public void Add(RouteItem item)
        {
            if (item.RouteNumber == "0")
            {
                return;
            }

            if (Items.ContainsKey(item.RouteNumber))
            {
                return;
            }

            Items.Add(item.RouteNumber, new()
            {
                Number = item.RouteNumber,
                Name = item.LongName,
                Stop1 = item.StopA,
                Stop2 = item.StopB,
            });
        }

        public override string ToString() => $"{nameof(Items.Count)}:{Items.Count} {nameof(Language)}:{Language}";
    }
}
