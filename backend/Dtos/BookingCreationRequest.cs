using System.ComponentModel.DataAnnotations;

using backend.Models;

namespace backend.Dtos
{
    public class BookingCreationRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [NotEmptyList]
        public List<TicketCreationRequest> Tickets { get; set; }

    }

    public class TicketCreationRequest
    {
        [Required]
        public PassengerCreationRequest Passenger { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "flightId must be greater than 0.")]
        public int FlightId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "flightClassId must be greater than 0.")]
        public int FlightClassId { get; set; }
    }
}
