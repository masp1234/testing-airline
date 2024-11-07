using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{

    [ApiController]
    [Route("/api/mysql/[controller]")]
    public class AirportsController(IAirportService airportService): ControllerBase
    {
        private readonly IAirportService _airportService = airportService;
        [HttpGet]
        public async Task<IActionResult> GetAllAirports()
        {
            try
            {
                var airports = await _airportService.GetAllAirports();
                return Ok(new { airports });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occured while trying to get airports." });
            }
        }
    }
}
