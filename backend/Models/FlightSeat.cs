using System;
using System.Collections.Generic;

namespace backend;

public partial class FlightSeat
{
    public int Id { get; set; }

    public int FlightId { get; set; }

    public int SeatId { get; set; }

    public virtual Flight Flight { get; set; } = null!;

    public virtual Seat Seat { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
