using Microsoft.AspNetCore.Mvc;

using BusTable.Core.Dto;
using BusTable.Core.Common;
using BusTable.Service.Services;

namespace BusTable.Service.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    public class BusDepartureTimesForTheRouteController : ControllerBase
    {
        private readonly ILogger<BusDepartureTimesForTheRouteController> _logger;
        private readonly DataTransferProviderService _dataTransferProviderService;

        public BusDepartureTimesForTheRouteController(
            ILogger<BusDepartureTimesForTheRouteController> logger,
            DataTransferProviderService dataTransferProviderService)
        {
            _logger = logger;
            _dataTransferProviderService = dataTransferProviderService;
        }

        [HttpGet]
        public ActionResult<BusDepartureTimeData> GetBusDepartureTimesForTheRoute(string language = "ANY", string routeNumber = "531")
        {
            /*
            // TODO: It must be in the Middleware
            if (!User.Identity.IsAuthenticated)
            {
                return Forbidden();
            }
            */


            BusDepartureTimeData? data;
            try { data = _dataTransferProviderService.GetBusDepartureTimesForTheRoute(language, routeNumber); }
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
