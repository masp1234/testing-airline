using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
	[Authorize (Roles = "Admin")]
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
				Console.WriteLine(ex);
				SentrySdk.CaptureException(ex);
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
				Console.WriteLine(ex);
				SentrySdk.CaptureException(ex);
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while trying to create a new flight." });
			}
		}

		[HttpGet("search")]
		public async Task<IActionResult> GetFlightsByDepartureDestinationAndDepartureDate([FromQuery] int departureAirportId,
																						  [FromQuery] int destinationAirportId,
																						  [FromQuery] DateOnly departureDate)
		{
			if (departureAirportId == 0 || destinationAirportId == 0 || departureDate == DateOnly.MinValue)
			{
				return BadRequest(new { message = "The request is missing either departureAirportId, destinationAirportId, departureDate or all of these parameters." });
			}
			try
			{
				var flights = await _flightService.GetFlightsByDepartureDestinationAndDepartureDate(departureAirportId, destinationAirportId, departureDate);
				return Ok(new { flights });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occured while trying to get flights by departure, destination and departure date." });
			}
		}

		// Dummy endpoint to test email sending
		// Sends both the 'cancellation' and 'change' email
		[HttpGet("cancelFlight")]
		public async Task<ActionResult> DummyCancelFlight()
		{
			try
			{
				await _flightService.CancelFlight();
				await _flightService.ChangeFlight();
				return Ok(new { message = "Email(s) has been sendt regarding cancellation of flight" });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occured while trying to cancel flight." });
			}
		}

	}
}
