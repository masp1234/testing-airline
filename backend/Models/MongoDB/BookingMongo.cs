using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace backend.Models.MongoDB
{
    [Collection("bookings")]
    public class BookingMongo
    {
        [BsonId]
        public long Id { get; set; }

        [BsonElement("confirmationNumber")]
        public string ConfirmationNumber { get; set; } = null!;

        [BsonElement("user")]
        public UserSnapshot User { get; set; } = null!;

        [BsonElement("tickets")]
        public List<TicketEmbedded> Tickets { get; set; }
    }

    public class UserSnapshot
    {
        [BsonElement("id")]
        public long Id { get; set; }

        [BsonElement("email")]
        public string Email { get; set; } = null!;

        [BsonElement("role")]
        public UserRole Role { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }
    }

    public class TicketEmbedded
    {
        [BsonElement("id")]
        public long Id { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("bookingId")]
        public long TicketsBookingId { get; set; }

        [BsonElement("ticketNumber")]
        public string TicketNumber { get; set; } = null!;

        [BsonElement("flight")]
        public FlightSnapShot Flight { get; set; }

        [BsonElement("passenger")]
        public PassengerEmbedded Passenger { get; set; } = null!;

        [BsonElement("flightClass")]
        public FlightClassSnapshot FlightClass { get; set; } = null!;
    }

    public class PassengerEmbedded
    {
        [BsonElement("id")]
        public long Id { get; set; }

        [BsonElement("firstName")]
        public string FirstName { get; set; } = null!;

        [BsonElement("lastName")]
        public string LastName { get; set; } = null!;

        [BsonElement("email")]
        public string Email { get; set; } = null!;
    }

    public class FlightClassSnapshot
    {
        [BsonElement("id")]
        public long Id { get; set; }

        [BsonElement("name")]
        public FlightClassName Name { get; set; }

        [BsonElement("priceMultiplier")]
        public decimal PriceMultiplier { get; set; }
    }

    public class FlightSnapShot
    {
        [BsonElement("id")]
        public long Id { get; set; }

        [BsonElement("flightCode")]
        public string FlightCode { get; set; }

        [BsonElement("departureTime")]
        public DateTime DepartureTime { get; set; }

        [BsonElement("completionTime")]
        public DateTime CompletionTime { get; set; }

        [BsonElement("travelTime")]
        public int TravelTime { get; set; }

        [BsonElement("kilometers")]
        public int? Kilometers { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("economyClassSeatsAvailable")]
        public int EconomyClassSeatsAvailable { get; set; }

        [BsonElement("businessClassSeatsAvailable")]
        public int BusinessClassSeatsAvailable { get; set; }

        [BsonElement("firstClassSeatsAvailable")]
        public int FirstClassSeatsAvailable { get; set; }

        [BsonElement("arrivalPort")]
        public AirportSnapshot ArrivalPort { get; set; } = null!;

        [BsonElement("departurePort")]
        public AirportSnapshot DeparturePort { get; set; } = null!;

        [BsonElement("flightsAirline")]
        public AirlineSnapshot FlightsAirline { get; set; } = null!;

        [BsonElement("flightsAirplane")]
        public AirplaneSnapshot FlightsAirplane { get; set; } = null!;
    }
}
