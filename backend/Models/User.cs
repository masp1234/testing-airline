using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class User
{
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public UserRole Role { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

public enum UserRole
{
    Customer,
    Admin
}
