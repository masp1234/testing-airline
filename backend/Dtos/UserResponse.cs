using backend.Models;

namespace backend.Dtos
{
    public class UserResponse
    {
        public string Email { get; set; }

        public string Role { get; set;}

        public List<Booking> Bookings { get; set; }
    }
}
