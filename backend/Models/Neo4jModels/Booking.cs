using Newtonsoft.Json;

namespace backend.Models.Neo4jModels
{
    public class Neo4jBooking
    {
        [JsonProperty("id")]
        public long Id { get; set; } 

        [JsonProperty("confirmation_number")]
        public string ConfirmationNumber { get; set; } = null!;

        [JsonProperty("code")]
        public string Code { get; set; } = null!;

        [JsonProperty("user_id")]
        public long UserId { get; set; }
        public virtual ICollection<Neo4jTicket> Tickets { get; set; } = new List<Neo4jTicket>();

        public virtual Neo4jUser User { get; set; } = null!;

    }
}