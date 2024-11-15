using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{

    [ApiController]
    [Route("/api/mysql/[controller]")]
    public class AirplanesController(IAirplaneService airplaneService) : ControllerBase
    {
        private readonly IAirplaneService _airplaneService = airplaneService;
        [HttpGet]
        public async Task<IActionResult> GetAllAirplanes()
        {
            try
            {
                var airplanes = await _airplaneService.GetAllAirplanes();
                return Ok(new { airplanes });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occured while trying to get airplanes." });
            }

        }
    }
}
