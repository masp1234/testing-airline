using Newtonsoft.Json;

namespace backend.Models.Neo4jModels
{
    public class Neo4jCity
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        public virtual ICollection<Neo4jAirport> Airports { get; set; } = new List<Neo4jAirport>();

        public virtual Neo4jState State { get; set; } = null!;

    }
}