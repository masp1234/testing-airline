using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Ticket
{
    public long Id { get; set; }

    public decimal Price { get; set; }

    public string TicketNumber { get; set; } = null!;

    public long PassengerId { get; set; }

    public long FlightId { get; set; }

    public long TicketsBookingId { get; set; }

    public long FlightClassId { get; set; }


    public virtual Flight Flight { get; set; } = null!;

    public virtual Passenger Passenger { get; set; } = null!;

    public virtual Booking TicketsBooking { get; set; } = null!;

    public virtual FlightClass FlightClass { get; set; } = null!;
}
