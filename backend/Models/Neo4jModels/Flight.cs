using Newtonsoft.Json;

namespace backend.Models.Neo4jModels
{
    public class Neo4jFlight
    {
        [JsonProperty("id")]
        public long Id { get; set; } 

        [JsonProperty("flight_code")]
        public string FlightCode { get; set; } = null!;

        [JsonProperty("departure_port")]
        public long DeparturePort { get; set; }

        [JsonProperty("arrival_port")]
        public long ArrivalPort { get; set; }

        [JsonProperty("departure_time")]
        public DateTime DepartureTime { get; set; }

        [JsonProperty("completion_time")]
        public DateTime CompletionTime { get; set; }

        [JsonProperty("travel_time")]
        public int TravelTime { get; set; }

        [JsonProperty("kilometers")]
        public int? Kilometers { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("economy_class_seats_available")]
        public int EconomyClassSeatsAvailable { get; set; }

        [JsonProperty("first_class_seats_available")]
        public int FirstClassSeatsAvailable { get; set; }

        [JsonProperty("business_class_seats_available")]
        public int BusinessClassSeatsAvailable { get; set; }

        [JsonProperty("flights_airline_id")]
        public long FlightsAirlineId { get; set; }

        [JsonProperty("flights_airplane_id")]
        public long FlightsAirplaneId { get; set; }

        [JsonProperty("idempotency_key")]
        public string? IdempotencyKey { get; set; }

        [JsonProperty("created_by")]
        public string? CreatedBy { get; set; }


        public virtual Neo4jAirport ArrivalPortNavigation { get; set; } = null!;

        public virtual Neo4jAirport DeparturePortNavigation { get; set; } = null!;

        public virtual Neo4jAirline FlightsAirline { get; set; } = null!;

        public virtual Neo4jAirplane FlightsAirplane { get; set; } = null!;

        public virtual ICollection<Neo4jTicket> Tickets { get; set; } = new List<Neo4jTicket>();

        public void DecrementSeatAvailability(FlightClassName flightClassName)
        {
            switch (flightClassName)
            {
                case FlightClassName.EconomyClass:
                    if (EconomyClassSeatsAvailable > 0)
                        EconomyClassSeatsAvailable--;
                    else
                        throw new InvalidOperationException("No Economy class seats available.");
                    break;

                case FlightClassName.BusinessClass:
                    if (BusinessClassSeatsAvailable > 0)
                        BusinessClassSeatsAvailable--;
                    else
                        throw new InvalidOperationException("No Business class seats available.");
                    break;

                case FlightClassName.FirstClass:
                    if (FirstClassSeatsAvailable > 0)
                        FirstClassSeatsAvailable--;
                    else
                        throw new InvalidOperationException("No First Class seats available.");
                    break;

                default:
                    throw new ArgumentException($"Invalid flight class: {flightClassName}");
            }
        }



    }
}
