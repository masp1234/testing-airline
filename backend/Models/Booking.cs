using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Booking
{
    public int Id { get; set; }

    public string ConfirmationNumber { get; set; } = null!;

    public int UserId { get; set; }


    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual User User { get; set; } = null!;
}
