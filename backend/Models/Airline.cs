using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Airline
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Airplane> Airplanes { get; set; } = new List<Airplane>();

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
