using backend.Dtos;

using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("/api/mysql/[controller]")]
    public class BookingsController: ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreationRequest bookingCreationRequest)
        {
            Console.WriteLine(bookingCreationRequest.Tickets[0].Passenger.Email);

            return Ok(bookingCreationRequest);
        }

    }
}
