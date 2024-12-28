using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Passenger
{
    public long Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = [];
}
