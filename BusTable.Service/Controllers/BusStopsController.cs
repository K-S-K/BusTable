using Microsoft.AspNetCore.Mvc;

using BusTable.Core.Dto;
using BusTable.Core.Common;
using BusTable.Service.Services;

namespace BusTable.Service.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    public class BusStopsController : Controller
    {
        private readonly ILogger<BusRoutesController> _logger;
        private readonly RouteService _routeService;

        public BusStopsController(
            ILogger<BusRoutesController> logger,
            RouteService routeService)
        {
            _logger = logger;
            _routeService = routeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusStopHeader>>> GetStops([FromQuery] BusStopsRequest request)
        {
            /*
            // TODO: It must be in the Middleware
            if (!User.Identity.IsAuthenticated)
            {
                return Forbidden();
            }
            */


            IEnumerable<BusStopHeader>? data;
            try { data = await _routeService.GetStops(request); }
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
