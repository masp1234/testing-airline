using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace backend.Models.MongoDB
{
    [Collection("flightClasses")]
    public class FlightClassMongo
    {
        [BsonId]
        public long Id { get; set; }

        [BsonElement("name")]
        public FlightClassName Name { get; set; }

        [BsonElement("priceMultiplier")]
        public decimal PriceMultiplier { get; set; }
    }
}
