using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class FlightClass
{
    public long Id { get; set; }

    public FlightClassName Name { get; set; }

    public decimal PriceMultiplier { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

public enum FlightClassName
{
    EconomyClass,
    BusinessClass,
    FirstClass
}
