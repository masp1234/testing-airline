using Newtonsoft.Json;

namespace backend.Models.Neo4jModels
{
    public class Neo4jPassenger
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; } = null!;

        [JsonProperty("last_name")]
        public string LastName { get; set; } = null!;

        [JsonProperty("email")]
        public string Email { get; set; } = null!;

        public virtual ICollection<Neo4jTicket> Tickets { get; set; } = [];
    }
}