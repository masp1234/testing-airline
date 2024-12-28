using backend.Models;

namespace backend.Dtos
{
    public class UserResponse
    {
        public long Id { get; set; }
        public string Email { get; set; }

        public string Role { get; set;}

        public List<Booking> Bookings { get; set; }
    }
}
