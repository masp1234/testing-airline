using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace backend.Models;

public partial class Airplane
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long AirplanesAirlineId { get; set; }

    public int EconomyClassSeats { get; set; }

    public int BusinessClassSeats { get; set; }

    public int FirstClassSeats { get; set; }

    public virtual Airline AirplanesAirline { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
