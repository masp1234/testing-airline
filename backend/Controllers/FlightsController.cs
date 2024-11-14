using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("/api/mysql/[controller]")]
    public class FlightsController(IFlightService flightService) : ControllerBase
    {
        private readonly IFlightService _flightService = flightService;

        [HttpGet]
        public async Task<IActionResult> GetAllFlights()
        {
            try
            {
                var flights = await _flightService.GetAllFlights();
                return Ok(flights);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occured while trying to get flights." });
            }
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
