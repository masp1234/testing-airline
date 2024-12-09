using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
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
				Console.WriteLine(ex);
				SentrySdk.CaptureException(ex);
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occured while trying to get flights." });
			}
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetFlightById(int id)
		{
			try
			{
				var flight = await _flightService.GetFlightWithRelationshipsById(id);
				if (flight == null)
				{
					return NotFound(new { message = $"No flight with ID '{id}' was found." });
				}
				return Ok(new { data = flight });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				SentrySdk.CaptureException(ex);
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"An error occured while trying to get existingFlight with ID {id}." });
			}
		}

		[Authorize(Roles = "Admin")]
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
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while trying to create the new flight." });
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

		[HttpPatch("{id}")]
		public async Task<IActionResult> UpdateFlight([FromBody]UpdateFlightRequest updateFlightRequest, int id)
		{
			try
			{
                Console.WriteLine(updateFlightRequest.DepartureDateTime);
                var existingFlight = await _flightService.GetFlightById(id);
                if (existingFlight == null)
                {
                    return NotFound(new { message = $"No flight with ID '{id}' was found." });
                }
                bool updatedSuccessfully = await _flightService.UpdateFlight(updateFlightRequest, existingFlight);
				if (!updatedSuccessfully)
				{
					return Conflict(new { message = $"Could not the update flight because there were conflicting flight schedules." });
				}
                return Ok(new { message = "The flight was updated successfully." });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                SentrySdk.CaptureException(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while trying to update flight." });
            }
        }

        // Dummy endpoint to test email sending
        // Sends both the 'cancellation' and 'change' email
        [HttpGet("emailTest")]
		public async Task<ActionResult> DummyCancelFlight()
		{
			try
			{
			//	await _flightService.CancelFlight();
				await _flightService.ChangeFlight();
				return Ok(new { message = "Email(s) has been sendt regarding cancellation of existingFlight." });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occured while trying to cancel existingFlight." });
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{Id}")]
		public async Task<IActionResult> DeleteFlight([FromRoute] int Id)
		{
			var flight = await _flightService.GetFlightById(Id);
			if (flight == null)
			{
    			return NotFound(new { message = $"Invalid flight ID provided. Flight with ID: {Id} does not exist." });
			}

			try
			{
				await _flightService.CancelFlight(Id);
				return Ok(new {message = $"Flight with ID: {Id} was deleted successfully!" });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while trying to delete a flight." });
			}
		}
	}
}
