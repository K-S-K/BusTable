using BusTable.Service.Controllers;
using Microsoft.Extensions.Logging;
using Moq;

namespace BusTable.Test
{
    [TestClass]
    public class LanguagesControllerTest
    {
        [TestMethod]
        public void ResultCount()
        {
            var logger = Mock.Of<ILogger<LanguagesController>>();
            LanguagesController ctrl = new(logger);

            var result = ctrl.Get();

            Assert.AreEqual(result.Count(), 4);
        }

        [TestMethod]
        public void ContainsAny()
        {
            var logger = Mock.Of<ILogger<LanguagesController>>();
            LanguagesController ctrl = new(logger);

            var result = ctrl.Get();

            Assert.IsTrue(result.Contains("ANY"));
        }
    }
}