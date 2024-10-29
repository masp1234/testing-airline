using System;
using System.Collections.Generic;
using backend.Models;

namespace backend;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public UserRole Role { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

public enum UserRole {
    Admin,
    Customer
}
