using System;
using System.Collections.Generic;
using backend.Models;

namespace backend;

public partial class State
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
