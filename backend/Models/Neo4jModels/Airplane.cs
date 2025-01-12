using Newtonsoft.Json;

namespace backend.Models.Neo4jModels
{
    public class Neo4jAirplane
    {
        [JsonProperty("id")]
        public long Id { get; set; } 

        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("airplanes_airline_id")]
        public long AirplanesAirlineId { get; set; }

        [JsonProperty("economy_class_seats")]
        public long EconomyClassSeats { get; set; }

        [JsonProperty("business_class_seats")]
        public long BusinessClassSeats { get; set; }

        [JsonProperty("first_class_seats")]
        public long FirstClassSeats { get; set; }

        public virtual Neo4jAirline AirplanesAirline { get; set; } = null!;

        public virtual ICollection<Neo4jFlight> Flights { get; set; } = new List<Neo4jFlight>();
    }
}