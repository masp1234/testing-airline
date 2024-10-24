using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Airplane
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int AirplanesAirlineId { get; set; }

    public virtual Airline AirplanesAirline { get; set; } = null!;

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
