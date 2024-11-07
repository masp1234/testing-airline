using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("/api/mysql/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightService _flightService;
        public FlightsController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        [HttpPost]
        public async Task<IActionResult> AddFlight([FromBody] FlightCreationRequest flightCreationRequest)
        {
            try
            {
                var createdFlight = await _flightService.CreateFlight(flightCreationRequest);
                return StatusCode(StatusCodes.Status201Created, new { message = "Flight was created successfully!", createdFlight });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occured while trying to create a new flight." });
            }
        }
    }
}
