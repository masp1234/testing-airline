using System;
using System.Collections.Generic;
using backend.Models;

namespace backend;

public partial class Invoice
{
    public int Id { get; set; }

    public double AmountDue { get; set; }

    public DateOnly DueDate { get; set; }

    public DateOnly? DatePaid { get; set; }

    public sbyte IsPaid { get; set; }

    public int InvoiceBookingId { get; set; }

    public virtual Booking InvoiceBooking { get; set; } = null!;
}
