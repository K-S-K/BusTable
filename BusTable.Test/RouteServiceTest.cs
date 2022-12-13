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

            RouteService service = new(
                new StopService(importService),
                new LanguageValidationService(),
                importService);


            BusRoutesRequest request = new();

            BusRouteData responce = service.GetRoutes(request).Result;

            Assert.AreEqual(5, responce.Count, $"{nameof(BusRoutesRequest)}: {request}");
        }

        [TestMethod]
        public void RoutesFilteringTest_Univer()
        {
            var logger = Mock.Of<ILogger<RouteService>>();

            RouteService service = new(
                new StopService(importService),
                new LanguageValidationService(),
                importService);


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

            RouteService service = new(
                new StopService(importService),
                new LanguageValidationService(),
                importService);


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
