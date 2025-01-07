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

        [HttpGet("{id}/bookedTimeSlots")]
        public async Task<IActionResult> GetBookedTimeslotsByAirplaneId(long id)
        {
            try
            {
                var airplane = await _airplaneService.GetAirplaneById(id);
                if (airplane == null)
                {
                    return NotFound(new { message = $"There was no airplane with ID: '{id}' found." });
                }

                var timeSlots = await _airplaneService.GetBookedTimeslotsByAirplaneId(id);
                return Ok(new { airplaneId = id, bookedTimeSlots = timeSlots });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occured while trying to find booked time slots." });
            }
        }
    }
}
