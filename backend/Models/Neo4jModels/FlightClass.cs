using Newtonsoft.Json;

namespace backend.Models.Neo4jModels
{
    public class Neo4jFlightClass
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public FlightClassName Name { get; set; }

        [JsonProperty("price_multiplier")]
        public decimal PriceMultiplier { get; set; }

        public virtual ICollection<Neo4jTicket> Tickets { get; set; } = new List<Neo4jTicket>();

    }
}

