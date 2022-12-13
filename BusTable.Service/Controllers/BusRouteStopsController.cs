using Microsoft.AspNetCore.Mvc;

using BusTable.Core.Dto;
using BusTable.Core.Common;
using BusTable.Service.Services;

namespace BusTable.Service.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    public class BusRouteStopsController : ControllerBase
    {
        private readonly ILogger<BusRouteStopsController> _logger;
        private readonly RouteService _routeService;

        public BusRouteStopsController(
            ILogger<BusRouteStopsController> logger,
            RouteService routeService)
        {
            _logger = logger;
            _routeService = routeService;
        }


        [HttpGet]
        public async Task<ActionResult<StopRouteSchedule>> GetRouteStops([FromQuery] BusRouteStopsRequest request)
        {
            /*
            // TODO: It must be in the Middleware
            if (!User.Identity.IsAuthenticated)
            {
                return Forbidden();
            }
            */

            /*
            // TODO: It must be in the Middleware
            try
            {
                _languageValidationService.Validate(request.Language);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
            */


            StopRouteSchedule? data;
            try { data = await _routeService.GetRouteStops(request); }
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
