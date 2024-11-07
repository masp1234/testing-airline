using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Seat
{
    public int Id { get; set; }

    public string Identifier { get; set; } = null!;

    public int AirplaneId { get; set; }

    public int FlightClassId { get; set; }

    public virtual Airplane Airplane { get; set; } = null!;

    public virtual FlightClass FlightClass { get; set; } = null!;
}
