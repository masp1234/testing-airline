using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Ticket
{
    public int Id { get; set; }

    public double Price { get; set; }

    public string TicketNumber { get; set; } = null!;

    public int PassengerId { get; set; }

    public int FlightId { get; set; }

    public int TicketsBookingId { get; set; }

    public int SeatId { get; set; }

    public virtual Flight Flight { get; set; } = null!;

    public virtual Passenger Passenger { get; set; } = null!;

    public virtual Booking TicketsBooking { get; set; } = null!;
}
