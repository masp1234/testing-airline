using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace backend.Models.MongoDB
{

    // Flight collection and embedded documents below FlightMongo. These have to be like this, since EF core will otherwise
    // try to create relationships with the other mongo collections, which is not supported in that way by the MongoDB EF Core provider.
    [Collection("flights")]
    public class FlightMongo
    {
        [BsonId]
        public long Id { get; set; }

        [BsonElement("flightCode")]
        public string FlightCode { get; set; } = null!;

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

        [BsonElement("idempotencyKey")]
        public string IdempotencyKey { get; set; }
    }

    public class AirportSnapshot
    {
        [BsonElement("id")]
        public long Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("code")]
        public string Code { get; set; } = null!;

        [BsonElement("city")]
        public CitySnapshot City { get; set; } = null!;
    }

    public class AirlineSnapshot
    {
        [BsonElement("id")]
        public long Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;
    }

    public class AirplaneSnapshot
    {
        [BsonElement("id")]
        public long Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("economyClassSeats")]
        public int EconomyClassSeats { get; set; }

        [BsonElement("businessClassSeats")]
        public int BusinessClassSeats { get; set; }

        [BsonElement("firstClassSeats")]
        public int FirstClassSeats { get; set; }
    }

    public class CitySnapshot
    {
        [BsonElement("id")]
        public long Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("state")]
        public StateSnapshot State { get; set; } = null!;
    }

    public class StateSnapshot
    {
        [BsonElement("id")]
        public long Id { get; set; }

        [BsonElement("code")]
        public string Code { get; set; } = null!;
    }

}
