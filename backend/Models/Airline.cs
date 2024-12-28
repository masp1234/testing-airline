using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace backend.Models;

public partial class Airline
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Airplane> Airplanes { get; set; } = new List<Airplane>();

    [JsonIgnore]
    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
