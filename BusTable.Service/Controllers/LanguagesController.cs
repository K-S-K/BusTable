using Microsoft.AspNetCore.Mvc;

using BusTable.Core.Dto;

namespace BusTable.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LanguagesController
    {
        private readonly ILogger<LanguagesController> _logger;

        public LanguagesController(ILogger<LanguagesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            // TODO: It must be from the Data Layer
            return new LanguageData().Languages;
        }
    }
}
