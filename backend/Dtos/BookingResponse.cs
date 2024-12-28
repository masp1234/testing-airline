using backend.Models;

namespace backend.Dtos
{
    public class BookingResponse
    {
        public long Id { get; set; }

        public string ConfirmationNumber { get; set; } = null!;

        public long UserId { get; set; }

        public List<TicketResponse> Tickets { get; set; } = new();
    }
}
