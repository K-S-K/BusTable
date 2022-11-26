using Microsoft.AspNetCore.Mvc;

using BusTable.Core.Dto;
using BusTable.Core.Common;
using BusTable.Service.Services;

namespace BusTable.Service.Controllers
{
    [ApiController]
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
        public ActionResult<BusDepartureTimeData> GetBusDepartureTimesForTheRoute(string language = "ANY", int lineId = 0)
        {
            /*
            // TODO: It must be in the Middleware
            if (!User.Identity.IsAuthenticated)
            {
                return Forbidden();
            }
            */


            BusDepartureTimeData? data;
            try { data = _dataTransferProviderService.GetBusDepartureTimesForTheRoute(language, lineId); }
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
