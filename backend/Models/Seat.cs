using System;
using System.Collections.Generic;
using backend.Models;

namespace backend;

public partial class Seat
{
    public int Id { get; set; }

    public string Identifier { get; set; } = null!;

    public int AirplaneId { get; set; }

    public virtual Airplane Airplane { get; set; } = null!;

    public virtual ICollection<FlightSeat> FlightSeats { get; set; } = new List<FlightSeat>();
}
