using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("/api/mysql/[controller]")]
    public class BookingsController(IBookingService bookingService): ControllerBase
    {
        private readonly IBookingService _bookingService = bookingService;
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreationRequest bookingCreationRequest)
        {
            try
            {
                var result = await _bookingService.CreateBooking(bookingCreationRequest);
                if (result.IsSucces)
                {
                    return Ok(result.Data);
                }
                else
                {
                    return NotFound(new { message = result.Message });
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occured while trying to create a booking." });
            }
        }

    }
}
