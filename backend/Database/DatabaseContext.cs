using System;
using System.Collections.Generic;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace backend.Database;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Airline> Airlines { get; set; }

    public virtual DbSet<Airplane> Airplanes { get; set; }

    public virtual DbSet<Airport> Airports { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<FlightClass> FlightClasses { get; set; }

    public virtual DbSet<Passenger> Passengers { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql(Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING"), Microsoft.EntityFrameworkCore.ServerVersion.Parse("9.0.1-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Airline>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("airlines");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Airplane>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("airplanes");

            entity.HasIndex(e => e.AirplanesAirlineId, "airline_id_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AirplanesAirlineId).HasColumnName("airplanes_airline_id");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");
            entity.Property(e => e.EconomyClassSeats).HasColumnName("economy_class_seats");
            entity.Property(e => e.BusinessClassSeats).HasColumnName("business_class_seats");
            entity.Property(e => e.FirstClassSeats).HasColumnName("first_class_seats");

            entity.HasOne(d => d.AirplanesAirline).WithMany(p => p.Airplanes)
                .HasForeignKey(d => d.AirplanesAirlineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("airplanes_airline_id");
        });

        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("airports");

            entity.HasIndex(e => e.CityId, "city_id_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .HasColumnName("code");
            entity.Property(e => e.Name)
                .HasMaxLength(120)
                .HasColumnName("name");

            entity.HasOne(d => d.City).WithMany(p => p.Airports)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("city_id");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("bookings");

            entity.HasIndex(e => e.UserId, "user_id_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConfirmationNumber)
                .HasMaxLength(45)
                .HasColumnName("confirmation_number");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("cities");

            entity.HasIndex(e => e.StateId, "state_id_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.StateId).HasColumnName("state_id");

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("state_id");
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("flights");

            entity.HasIndex(e => e.ArrivalPort, "arrival_port_idx");

            entity.HasIndex(e => e.DeparturePort, "departure_port_idx");

            entity.HasIndex(e => e.FlightsAirlineId, "flights_airline_id_idx");

            entity.HasIndex(e => e.FlightsAirplaneId, "flights_airplane_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ArrivalPort).HasColumnName("arrival_port");
            entity.Property(e => e.DeparturePort).HasColumnName("departure_port");
            entity.Property(e => e.DepartureTime)
                .HasColumnType("datetime")
                .HasColumnName("departure_time");
            entity.Property(e => e.CompletionTime)
                .HasColumnType("datetime")
                .HasColumnName("completion_time");
            entity.Property(e => e.FlightCode)
                .HasMaxLength(45)
                .HasColumnName("flight_code");
            entity.Property(e => e.FlightsAirlineId).HasColumnName("flights_airline_id");
            entity.Property(e => e.FlightsAirplaneId).HasColumnName("flights_airplane_id");
            entity.Property(e => e.Kilometers)
                .HasMaxLength(45)
                .HasColumnName("kilometers");

            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.EconomyClassSeatsAvailable).HasColumnName("economy_class_seats_available");
            entity.Property(e => e.BusinessClassSeatsAvailable).HasColumnName("business_class_seats_available");
            entity.Property(e => e.FirstClassSeatsAvailable).HasColumnName("first_class_seats_available");

            entity.Property(e => e.TravelTime).HasColumnName("travel_time");
            entity.Property(e => e.IdempotencyKey).HasColumnName("idempotency_key");

            entity.HasOne(d => d.ArrivalPortNavigation).WithMany(p => p.FlightArrivalPortNavigations)
                .HasForeignKey(d => d.ArrivalPort)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("arrival_port");

            entity.HasOne(d => d.DeparturePortNavigation).WithMany(p => p.FlightDeparturePortNavigations)
                .HasForeignKey(d => d.DeparturePort)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("departure_port");

            entity.HasOne(d => d.FlightsAirline).WithMany(p => p.Flights)
                .HasForeignKey(d => d.FlightsAirlineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("flights_airline_id");

            entity.HasOne(d => d.FlightsAirplane).WithMany(p => p.Flights)
                .HasForeignKey(d => d.FlightsAirplaneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("flights_airplane_id");
        });

        modelBuilder.Entity<FlightClass>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("flight_classes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PriceMultiplier)
                .HasColumnName("price_multiplier");
            entity.Property(e => e.Name)
                .HasConversion<string>()
                .HasColumnName("name");
        });

        modelBuilder.Entity<Passenger>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("passengers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(80)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(120)
                .HasColumnName("last_name");
            entity.Property(e => e.Email)
                .HasMaxLength(320)
                .HasColumnName("email");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("states");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(2)
                .HasColumnName("code");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tickets");

            entity.HasIndex(e => e.FlightId, "flight_id_idx");

            entity.HasIndex(e => e.PassengerId, "passenger_id_idx");

            entity.HasIndex(e => e.TicketsBookingId, "tickets_booking_id_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FlightId).HasColumnName("flight_id");
            entity.Property(e => e.PassengerId).HasColumnName("passenger_id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.FlightClassId).HasColumnName("tickets_class_id");
            entity.Property(e => e.TicketNumber)
                .HasMaxLength(45)
                .HasColumnName("ticket_number");
            entity.Property(e => e.TicketsBookingId).HasColumnName("tickets_booking_id");

            entity.HasOne(d => d.Flight).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("flight_id");

            entity.HasOne(d => d.Passenger).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.PassengerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("passenger_id");

            entity.HasOne(d => d.TicketsBooking).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.TicketsBookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tickets_booking_id");

            entity.HasOne(d => d.FlightClass).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.FlightClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tickets_class_id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "email_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(80)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasConversion<string>()
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
