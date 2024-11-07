using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class State
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
