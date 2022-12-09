using BusTable.Core.Dto;
using BusTable.Core.Import;
using BusTable.Core.Models;
using BusTable.Service.Services;
using BusTable.Test.Resources;
using System.Xml.Linq;

namespace BusTable.Test
{
    public class TestImportService : IImportService
    {
        public RouteRegistry LoadRouteRegistry()
        {
            RouteRegistry data = new()
            {
                Language = "ANY"
            };

            RouteList routeList = new(XElement.Parse(Source.data_route_list));

            routeList.Routes.Values.ToList().ForEach(x => data.Items.Add(x.RouteNumber, new()
            {
                Number = x.RouteNumber,
                Name = x.LongName,
                Stop1 = x.StopA,
                Stop2 = x.StopB,
            }));

            return data;
        }

        public StopData LoadRouteSchedule(string fileName, StopService stopDataService)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, StopData> LoadStopData(IEnumerable<string> routeIds, StopService stopDataService)
        {
            var stops = new Dictionary<string, StopData>();

            AddRoute(XElement.Parse(Source.data_schedule_304_f1));
            AddRoute(XElement.Parse(Source.data_schedule_310_f1));
            AddRoute(XElement.Parse(Source.data_schedule_310_f1));
            AddRoute(XElement.Parse(Source.data_schedule_383_f0));
            AddRoute(XElement.Parse(Source.data_schedule_383_f1));
            AddRoute(XElement.Parse(Source.data_schedule_387_f0));
            AddRoute(XElement.Parse(Source.data_schedule_387_f1));
            AddRoute(XElement.Parse(Source.data_schedule_531_f0));
            AddRoute(XElement.Parse(Source.data_schedule_531_f1));

            return stops;

            void AddRoute(XElement xe)
            {
                RouteSchedule schedule = new(xe);
                StopData stopData = ImportService.ApplyRouteSchedule(stopDataService, schedule);

                stops[stopData.RouteNumber] = stopData;
            }
        }


        public StopRegistry LoadStopRegistry()
        {
            StopRegistry data = new();

            return data;
        }
    }
}
