﻿using Microsoft.AspNetCore.Mvc;

using BusTable.Core.Dto;

namespace BusTable.Service.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
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
            // TODO: It must be from the Depency Injection
            return new LanguageData().Languages;
        }
    }
}
