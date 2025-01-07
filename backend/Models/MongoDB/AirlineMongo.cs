using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace backend.Models.MongoDB
{
    [Collection("airlines")]
    public class AirlineMongo
    {
        [BsonId]
        public long Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;

    }
}
