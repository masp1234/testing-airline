using backend.Models;

namespace backend.Dtos
{
    public class BookingCreationResponse
    {
        public int Id { get; set; }

        public string ConfirmationNumber { get; set; } = null!;

        public int UserId { get; set; }
    }
}
