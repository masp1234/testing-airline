using Newtonsoft.Json;

namespace backend.Models.Neo4jModels
{
    public class Neo4jAirline
    {
        [JsonProperty("id")]
        public long Id { get; set; } 

        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        public virtual ICollection<Neo4jAirplane> Airplanes { get; set; } = new List<Neo4jAirplane>();

        public virtual ICollection<Neo4jFlight> Flights { get; set; } = new List<Neo4jFlight>();
    }
}