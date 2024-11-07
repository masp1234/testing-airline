using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class FlightClass
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
