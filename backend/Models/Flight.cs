using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace backend.Models;

public partial class Flight
{
    public int Id { get; set; }

    public string FlightCode { get; set; } = null!;

    public int DeparturePort { get; set; }

    public int ArrivalPort { get; set; }

    public DateTime DepartureTime { get; set; }

    public int TravelTime { get; set; }

    public string? Kilometers { get; set; }

    public double Price { get; set; }

    public int EconomyClassSeatsAvailable { get; set; }
    public int FirstClassSeatsAvailable { get; set; }

    public int BusinessClassSeatsAvailable { get; set; }

    public int FlightsAirlineId { get; set; }

    public int FlightsAirplaneId { get; set; }

    public string IdempotencyKey { get; set; }
    [JsonIgnore]
    public virtual Airport ArrivalPortNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual Airport DeparturePortNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual Airline FlightsAirline { get; set; } = null!;
    [JsonIgnore]
    public virtual Airplane FlightsAirplane { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
