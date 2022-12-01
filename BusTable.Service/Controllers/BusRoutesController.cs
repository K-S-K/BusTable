using Microsoft.AspNetCore.Mvc;

using BusTable.Core.Dto;
using BusTable.Core.Common;
using BusTable.Service.Services;

namespace BusTable.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BusRoutesController : Controller
    {
        private readonly ILogger<BusRoutesController> _logger;
        private readonly DataTransferProviderService _dataTransferProviderService;

        public BusRoutesController(
            ILogger<BusRoutesController> logger,
            DataTransferProviderService dataTransferProviderService)
        {
            _logger = logger;
            _dataTransferProviderService = dataTransferProviderService;
        }

        [HttpGet]
        public async Task<ActionResult<BusRouteData>> GetRoutes([FromQuery] BusRoutesRequest request)
        {
            /*
            // TODO: It must be in the Middleware
            if (!User.Identity.IsAuthenticated)
            {
                return Forbidden();
            }
            */


            BusRouteData? data;
            try { data = await _dataTransferProviderService.GetRoutes(request); }
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
