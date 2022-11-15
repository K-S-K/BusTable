using Microsoft.AspNetCore.Mvc;

using BusTable.Core.Common;
using BusTable.Core.Models;
using BusTable.Service.Services;

namespace BusTable.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BusStopsController : Controller
    {
        private readonly ILogger<BusRoutesController> _logger;
        private readonly StopService _stopDataService;

        public BusStopsController(
            ILogger<BusRoutesController> logger,
            StopService stopDataService)
        {
            _logger = logger;
            _stopDataService = stopDataService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<StopHeader>> GetStops(string language = "ANY", double lat = 0.0, double lon = 0.0)
        {
            /*
            // TODO: It must be in the conveyer
            if (!User.Identity.IsAuthenticated)
            {
                return Forbidden();
            }
            */


            IEnumerable<StopHeader>? data;
            try { data = _stopDataService.GetStops(language, lat, lon); }
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
