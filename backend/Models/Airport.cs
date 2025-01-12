using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace backend.Models;

public partial class Airport
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public long CityId { get; set; }

    public virtual City? City { get; set; }

    [JsonIgnore]
    public virtual ICollection<Flight> FlightArrivalPortNavigations { get; set; } = new List<Flight>();

    [JsonIgnore]
    public virtual ICollection<Flight> FlightDeparturePortNavigations { get; set; } = new List<Flight>();
}
