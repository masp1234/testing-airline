using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace backend.Models;

public partial class Airplane
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int AirplanesAirlineId { get; set; }

    public virtual Airline AirplanesAirline { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
