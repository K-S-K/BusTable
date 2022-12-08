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
        public void FilteringTest_All()
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
        public void FilteringTest_Univer()
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

    }
}
