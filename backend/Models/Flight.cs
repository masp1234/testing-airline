using System;
using System.Collections.Generic;
using backend.Models;

namespace backend;

public partial class Flight
{
    public int Id { get; set; }

    public string FlightCode { get; set; } = null!;

    public int DeparturePort { get; set; }

    public int ArrivalPort { get; set; }

    public DateTime DepartureTime { get; set; }

    public int TravelTime { get; set; }

    public string? Kilometers { get; set; }

    public int FlightsAirlineId { get; set; }

    public int FlightsAirplaneId { get; set; }

    public virtual Airplane Airplane { get; set; } = null;

    public virtual Airport ArrivalPortNavigation { get; set; } = null!;

    public virtual Airport DeparturePortNavigation { get; set; } = null!;

    public virtual ICollection<FlightSeat> FlightSeats { get; set; } = new List<FlightSeat>();

    public virtual Airline FlightsAirline { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
