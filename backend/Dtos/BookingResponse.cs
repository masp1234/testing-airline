using backend.Models;

namespace backend.Dtos
{
    public class BookingResponse
    {
        public int Id { get; set; }

        public string ConfirmationNumber { get; set; } = null!;

        public int UserId { get; set; }

        public List<TicketResponse> Tickets { get; set; } = new();
    }
}
