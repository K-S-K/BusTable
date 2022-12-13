using Microsoft.AspNetCore.Mvc;

using BusTable.Core.Common;
using BusTable.Core.Models;
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
        public ActionResult<IEnumerable<StopHeader>> GetStops(string language = "ANY", double lat = 0.0, double lon = 0.0)
        {
            /*
            // TODO: It must be in the Middleware
            if (!User.Identity.IsAuthenticated)
            {
                return Forbidden();
            }
            */


            IEnumerable<StopHeader>? data;
            try { data = _routeService.GetStops(language, lat, lon); }
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
