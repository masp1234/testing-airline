using Newtonsoft.Json;

namespace backend.Models.Neo4jModels
{
    public class Neo4jUser
    {
        [JsonProperty("id")]
        public long Id { get; set; } 

        [JsonProperty("email")]
        public string Email { get; set; } = null!;

        [JsonProperty("password")]
        public string Password { get; set; } = null!;

        [JsonProperty("role")]
        public string Role { get; set; } = null!;

        public virtual ICollection<Neo4jBooking> Bookings { get; set; } = new List<Neo4jBooking>();
    }

    
}