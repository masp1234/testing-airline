using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace backend.Models.MongoDB
{
    [Collection("airplanes")]
    public class AirplaneMongo
    {
        [BsonId]
        public long Id { get; set; }


        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("airlineId")]
        public long AirplanesAirlineId { get; set; }

        [BsonElement("economyClassSeats")]
        public long EconomyClassSeats { get; set; }

        [BsonElement("businessClassSeats")]
        public long BusinessClassSeats { get; set; }

        [BsonElement("firstClassSeats")]
        public long FirstClassSeats { get; set; }
    }
}
