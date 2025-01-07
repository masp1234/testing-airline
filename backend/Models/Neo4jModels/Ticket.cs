using Newtonsoft.Json;

namespace backend.Models.Neo4jModels
{
    public class Neo4jTicket
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
        
        [JsonProperty("ticket_number")]
        public string TicketNumber { get; set; } = null!;

        [JsonProperty("passenger_id")]
        public long PassengerId { get; set; }

        [JsonProperty("flight_id")]
        public long FlightId { get; set; }

        [JsonProperty("tickets_booking_id")]
        public long TicketsBookingId { get; set; }


        [JsonProperty("flight_class_id")]
        public long FlightClassId { get; set; }


        public virtual Neo4jFlight Flight { get; set; } = null!;

        public virtual Neo4jPassenger Passenger { get; set; } = null!;

        public virtual Neo4jBooking TicketsBooking { get; set; } = null!;

        public virtual Neo4jFlightClass FlightClass { get; set; } = null!;
    }
}