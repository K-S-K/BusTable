﻿using Microsoft.AspNetCore.Mvc;

using BusTable.Service.Services;
using BusTable.Core.Dto;
using BusTable.Core.Common;

namespace BusTable.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BusRouteStopsController : ControllerBase
    {
        private readonly ILogger<BusRouteStopsController> _logger;
        private readonly DataTransferProviderService _dataTransferProviderService;

        public BusRouteStopsController(
            ILogger<BusRouteStopsController> logger,
            DataTransferProviderService dataTransferProviderService)
        {
            _logger = logger;
            _dataTransferProviderService = dataTransferProviderService;
        }


        [HttpGet]
        public ActionResult<StopData> GetRoutePoints(string language = "ANY", int routeId = 531, int cityId = 0)
        {
            /*
            // TODO: It must be in the conveyer
            if (!User.Identity.IsAuthenticated)
            {
                return Forbidden();
            }
            */

            StopData? data;
            try { data = _dataTransferProviderService.GetRoutePoints(language, routeId, cityId); }
            catch (BadRequestException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return Problem(ex.Message); }

            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }
    }
}
