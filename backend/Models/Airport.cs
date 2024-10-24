using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Airport
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int? CityId { get; set; }

    public virtual City? City { get; set; }

    public virtual ICollection<Flight> FlightArrivalPortNavigations { get; set; } = new List<Flight>();

    public virtual ICollection<Flight> FlightDeparturePortNavigations { get; set; } = new List<Flight>();
}
