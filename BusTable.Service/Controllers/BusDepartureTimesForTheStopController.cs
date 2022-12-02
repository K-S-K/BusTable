using Microsoft.AspNetCore.Mvc;

using BusTable.Core.Dto;
using BusTable.Core.Common;
using BusTable.Service.Services;

namespace BusTable.Service.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
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
        public async Task<ActionResult<BusDepartureTimeData>> GetBusDepartureTimesForTheStop([FromQuery] BusDepartureTimesForTheStopRequest request)
        {
            /*
            // TODO: It must be in the Middleware
            if (!User.Identity.IsAuthenticated)
            {
                return Forbidden();
            }
            */


            BusDepartureTimeData? data;
            try { data = await _dataTransferProviderService.GetBusDepartureTimesForTheStop(request); }
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
