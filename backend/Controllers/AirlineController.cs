using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{

    [ApiController]
    [Route("/api/mysql/[controller]")]
    public class AirlinesController(IAirlineService airlineService): ControllerBase
    {
        private readonly IAirlineService _airlineService = airlineService;
        [HttpGet]
        public async Task<IActionResult> GetAllAirports()
        {
            try
            {
                var airlines = await _airlineService.GetAllAirlines();
                return Ok(new { airlines });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occured while trying to get airlines." });
            }
        }
    }
}