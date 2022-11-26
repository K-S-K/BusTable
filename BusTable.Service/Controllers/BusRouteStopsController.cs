﻿using Microsoft.AspNetCore.Mvc;

using BusTable.Core.Dto;
using BusTable.Core.Common;
using BusTable.Service.Services;

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
        public ActionResult<StopData> GetRouteStops(string language = "ANY", string routeNumber = "531", int cityId = 0)
        {
            /*
            // TODO: It must be in the Middleware
            if (!User.Identity.IsAuthenticated)
            {
                return Forbidden();
            }
            */

            StopData? data;
            try { data = _dataTransferProviderService.GetRouteStops(language, routeNumber, cityId); }
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