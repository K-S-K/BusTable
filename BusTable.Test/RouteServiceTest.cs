using BusTable.Core.Dto;
using BusTable.Service.Controllers;
using BusTable.Service.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace BusTable.Test
{
    [TestClass]
    public class RouteServiceTest
    {
        private IImportService importService = new TestImportService();

        [TestMethod]
        public void RoutesFilteringTest_All()
        {
            var logger = Mock.Of<ILogger<RouteService>>();

            RouteService service = new(importService);

            BusRoutesRequest request = new();

            BusRouteData responce = service.GetRoutes(request).Result;

            Assert.AreEqual(5, responce.Count, $"{nameof(BusRoutesRequest)}: {request}");
        }

        [TestMethod]
        public void RoutesPagingTest()
        {
            var logger = Mock.Of<ILogger<RouteService>>();

            RouteService service = new(importService);

            BusRoutesRequest request1 = new() { PageNumber = 1, PageSize = 3 };
            BusRoutesRequest request2 = new() { PageNumber = 2, PageSize = 3 };

            BusRouteData responce1 = service.GetRoutes(request1).Result;
            BusRouteData responce2 = service.GetRoutes(request2).Result;

            Assert.AreEqual(3, responce1.Count, $"{nameof(BusRoutesRequest)}: {request1}");
            Assert.AreEqual(2, responce2.Count, $"{nameof(BusRoutesRequest)}: {request2}");
        }

        [TestMethod]
        public void RoutesFilteringTest_Univer()
        {
            var logger = Mock.Of<ILogger<RouteService>>();

            RouteService service = new(importService);

            BusRoutesRequest request = new()
            {
                Search = "Univer"
            };

            BusRouteData responce = service.GetRoutes(request).Result;

            Assert.AreEqual(3, responce.Count, $"{nameof(BusRoutesRequest)}: {request}");
        }


        [TestMethod]
        public void GetDepartureTimesTest()
        {
            var logger = Mock.Of<ILogger<RouteService>>();

            RouteService service = new(importService);

            BusDepartureTimesForTheStopRequest request = new()
            {
                RouteNumber = "531",
                StopID = 3899
            };

            BusDepartureTimeData? responce = service.GetBusDepartureTimesForTheStop(request).Result;

            Assert.IsNotNull(responce);

            Assert.AreEqual(responce.StopName, "Abashidze St 78", nameof(responce.StopName));

            Assert.AreEqual(63, responce?.Count, $"{nameof(BusRoutesRequest)}: {request}");
        }
    }
}
