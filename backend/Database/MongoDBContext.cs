using backend.Models.MongoDB;
using Microsoft.EntityFrameworkCore;

namespace backend.Database
{
    public class MongoDBContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<AirplaneMongo> Airplanes { get; set; }

        public DbSet<FlightMongo> Flights { get; set; }

        public DbSet<AirlineMongo> Airlines { get; set; }

        public DbSet<AirportMongo> Airports { get; set; }

        public DbSet<UserMongo> Users { get; set; }
        public DbSet<FlightClassMongo> FlightClasses { get; set; }

        public DbSet<BookingMongo> Bookings { get; set; }
        }
    }

