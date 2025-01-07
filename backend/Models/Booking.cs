using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Booking
{
    public long Id { get; set; }

    public string ConfirmationNumber { get; set; } = null!;

    public long UserId { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual User User { get; set; } = null!;
}
