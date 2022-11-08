using Microsoft.AspNetCore.Mvc;

using BusTable.Core.Dto;
using BusTable.Core.Common;
using BusTable.Service.Services;

namespace BusTable.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BusDepartureTimesForTheStopController : ControllerBase
    {
        private readonly ILogger<BusDepartureTimesForTheStopController> _logger;
        private readonly DataTransferProviderService _dataTransferProviderService;

        public BusDepartureTimesForTheStopController(
            ILogger<BusDepartureTimesForTheStopController> logger,
            DataTransferProviderService dataTransferProviderService)
        {
            _logger = logger;
            _dataTransferProviderService = dataTransferProviderService;
        }

        [HttpGet]
        public ActionResult<BusDepartureTimeData> GetBusDepartureTimesForThePoint(string language = "ANY", int stopId = 0)
        {
            /*
            // TODO: It must be in the conveyer
            if (!User.Identity.IsAuthenticated)
            {
                return Forbidden();
            }
            */


            BusDepartureTimeData? data;
            try { data = _dataTransferProviderService.GetBusDepartureTimesForTheStop(language, stopId); }
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
