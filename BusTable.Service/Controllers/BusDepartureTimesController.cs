using Microsoft.AspNetCore.Mvc;

using BusTable.Core.Dto;
using BusTable.Core.Common;
using BusTable.Service.Services;

namespace BusTable.Service.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    public class BusDepartureTimesController : ControllerBase
    {
        private readonly ILogger<BusDepartureTimesController> _logger;
        private readonly RouteService _routeService;

        public BusDepartureTimesController(
            ILogger<BusDepartureTimesController> logger,
            RouteService routeService
)
        {
            _logger = logger;
            _routeService = routeService;
        }

        [HttpGet]
        public async Task<ActionResult<BusDepartureTimeData>> GetBusDepartureTimes([FromQuery] BusDepartureTimesForTheStopRequest request)
        {
            /*
            // TODO: It must be in the Middleware
            if (!User.Identity.IsAuthenticated)
            {
                return Forbidden();
            }
            */


            BusDepartureTimeData? data;
            try { data = await _routeService.GetBusDepartureTimesForTheStop(request); }
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
