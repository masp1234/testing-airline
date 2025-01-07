using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace backend.Models.MongoDB
{
    [Collection("airports")]
    public class AirportMongo
    {
        [BsonId]
        public long Id { get; set; }


        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("code")]
        public string Code { get; set; } = null!;

        [BsonElement("city")]
        public virtual CitySnapshot City { get; set; }
    }
}
