using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("/api/mysql/[controller]")]
    public class FlightsController: ControllerBase
    {
        private readonly IFlightService _flightService;
        public FlightsController(IFlightService flightService) {
            _flightService = flightService;
                }

        [HttpPost]
        public async Task<IActionResult> AddFlight([FromBody] FlightCreationRequest flightCreationRequest)
        {
            await _flightService.CreateFlight(flightCreationRequest);

            return StatusCode(StatusCodes.Status201Created, new { message = "Flight was created successfully!" });
        }

    }
}
