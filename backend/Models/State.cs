using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace backend.Models;

public partial class State
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
