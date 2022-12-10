using System.Xml.Linq;

using BusTable.Core.Dto;
using BusTable.Core.Import;
using BusTable.Core.Models;
using BusTable.Test.Resources;
using BusTable.Service.Services;

namespace BusTable.Test
{
    public class TestImportService : ImportService, IImportService
    {
        public TestImportService() : base(null)
        {
        }

        public new RouteRegistry LoadRouteRegistry()
        {
            RouteRegistry data = new()
            {
                Language = "ANY"
            };

            RouteList routeList = new(XElement.Parse(Source.data_route_list));

            routeList.Routes.Values.ToList().ForEach(x => data.Add(x));

            return data;
        }

        public new StopRouteSchedule LoadRouteSchedule(string fileName, StopService stopDataService)
        {
            throw new NotImplementedException();
        }

        public new Dictionary<string, StopRouteSchedule> LoadStopData(IEnumerable<string> routeIds, StopService stopDataService)
        {
            var stops = new Dictionary<string, StopRouteSchedule>();

            AddRoute(XElement.Parse(Source.data_schedule_304_f0));
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
                StopRouteSchedule stopData = ApplyRouteSchedule(stopDataService, schedule);

                stops[stopData.RouteNumber] = stopData;
            }
        }


        public new StopRegistry LoadStopRegistry()
        {
            StopRegistry data = new();

            return data;
        }
    }
}
