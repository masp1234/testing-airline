using backend.Models;

namespace backend.Dtos
{
    public class FlightResponse
    {
        public long Id { get; set; }

        public string FlightCode { get; set; } = null!;


        public DateTime DepartureTime { get; set; }

        public DateTime CompletionTime { get; set; }

        public int TravelTime { get; set; }

        public string? Kilometers { get; set; }

        public double Price { get; set; }

        public int EconomyClassSeatsAvailable { get; set; }
        public int FirstClassSeatsAvailable { get; set; }
        public int BusinessClassSeatsAvailable { get; set; }

        public virtual Airport ArrivalPortNavigation { get; set; } = null!;

        public virtual Airport DeparturePortNavigation { get; set; } = null!;

        public virtual Airline FlightsAirline { get; set; } = null!;

        public virtual Airplane FlightsAirplane { get; set; } = null!;

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
