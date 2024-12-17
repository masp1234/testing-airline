using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("/api/mysql/[controller]")]
    public class BookingsController(IBookingService bookingService): ControllerBase
    {

        private readonly IBookingService _bookingService = bookingService;

        [Authorize(Roles = "Customer")]
        // Starting the route with a slash ignores the controller-level route prefix.
        [HttpGet("/api/mysql/users/{email}/bookings")]
        public async Task<IActionResult> GetBookingsByUserEmail(string email)
        {
            try
            {
                // Checks that the JWT contains the same email that you used as a path variable. If not, you can't access the bookings.
                var emailClaim = User.Claims.FirstOrDefault(claim => claim.Value == email);
                if (emailClaim == null)
                {
                    Console.WriteLine("You can only see your own booking");
                    return Unauthorized(new { message = "You are only allowed to see your own bookings." });
                }

                var bookings = await _bookingService.GetBookingsByUserEmail(email);
                return Ok(new { data = bookings.Data });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { message = "An error occured while trying to get bookings." });
            }
        }
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreationRequest bookingCreationRequest)
        {
            /*
             * Checks that the email in the request body is the same as the users email claim.
             * This is done, so that a customer cannot create an order using another customer's email.
            */
            var emailClaim = User.Claims.FirstOrDefault(claim => claim.Value == bookingCreationRequest.Email);
            if (emailClaim == null)
            {
                Console.WriteLine("You can only create bookings with your own email address");
                return Unauthorized();
            }
            try
            {   
                var result = await _bookingService.CreateBooking(bookingCreationRequest);
                if (result.IsSucces)
                {
                    return StatusCode(201, new { message = result.Message, createdBooking = result.Data});
                }
                else
                {
                    return NotFound(new { message = result.Message });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                SentrySdk.CaptureException(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occured while trying to create a booking." });
            }
        }

    }
}
