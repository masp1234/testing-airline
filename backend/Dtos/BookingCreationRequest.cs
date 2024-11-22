using System.ComponentModel.DataAnnotations;

using backend.Models;

namespace backend.Dtos
{
    public class BookingCreationRequest
    {
        public int UserId { get; set; }

        public TicketCreationRequest[] Tickets { get; set; }

    }

    public class TicketCreationRequest
    {
        public PassengerCreationRequest Passenger { get; set; }

        public int FlightId { get; set; }

        public int FlightClassId { get; set; }

    }

    public class PassengerCreationRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }
}
