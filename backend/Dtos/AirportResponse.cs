using backend.Models;

namespace backend.Dtos
{
    public class AirportResponse
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public virtual City City { get; set; }
    }
}
