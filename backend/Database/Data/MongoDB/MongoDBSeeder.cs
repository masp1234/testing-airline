using backend.Models;
using backend.Models.MongoDB;
using MongoDB.Driver;


namespace backend.Database.Data.MongoDB
{
    public class MongoDBSeeder(MongoDBContext context, IMongoClient client)
    {

        private readonly MongoDBContext _context = context;
        private readonly IMongoClient _client = client;

        public void Seed()
        {
            var database = _client.GetDatabase("mydatabase");
            var collection = database.GetCollection<BookingMongo>("bookings");
            // Create multikey index tickets.flight.id - is used when you need to find tickets by flight id (when updating a flight for example)
            var indexKeysDefinition = Builders<BookingMongo>.IndexKeys.Ascending("tickets.flight.id");
            collection.Indexes.CreateOne(new CreateIndexModel<BookingMongo>(indexKeysDefinition));

            if (!_context.Airplanes.Any())
            {

                var airlines = new List<AirlineMongo>()
                {
                    new()
                    {
                        Id = 1,
                        Name = "Delta Airlines"
                    }
                };

                var airplanes = new List<AirplaneMongo>()
                {
                  new()
                  {
                      Id = 1,
                      Name = "Boeing 2",
                      AirplanesAirlineId = 1,
                      EconomyClassSeats = 200,
                      BusinessClassSeats = 40,
                      FirstClassSeats = 20
                  },
                  new()
                  {
                      Id = 2,
                      Name = "Boeing 223",
                      AirplanesAirlineId = 1,
                      EconomyClassSeats = 222,
                      BusinessClassSeats = 43,
                      FirstClassSeats = 32
                  },
                  new()
                  {
                      Id = 3,
                      Name = "Boeing 111",
                      AirplanesAirlineId = 1,
                      EconomyClassSeats = 200,
                      BusinessClassSeats = 21,
                      FirstClassSeats = 12
                  }
                };

                var flights = new List<FlightMongo>()
                {
                    new()
                    {
                        Id = 1,
                        FlightCode = "TEST123",
                        DepartureTime = new DateTime(),
                        CompletionTime = new DateTime().AddHours(5),
                        TravelTime = 200,
                        Kilometers = 2500,
                        Price = 300,
                        EconomyClassSeatsAvailable = 100,
                        BusinessClassSeatsAvailable = 40,
                        FirstClassSeatsAvailable = 10,
                        IdempotencyKey = "123344324",
                        FlightsAirline = new()
                        {
                            Id = 1,
                            Name = "Delta Airlines"
                        },
                        FlightsAirplane = new()
                        {
                            Id = 1,
                            Name = "Test plane",
                            EconomyClassSeats = 100,
                            BusinessClassSeats = 40,
                            FirstClassSeats = 10
                        },
                        ArrivalPort = new()
                        {
                            Id = 1,
                            Name = "Los angeles airport",
                            Code = "LAX",
                            City = new()
                            {
                                Id = 1,
                                Name = "Los Angeles",
                                State = new()
                                {
                                    Id = 1,
                                    Code = "CA"
                                }
                            }
                        },
                        DeparturePort = new()
                        {
                            Id = 2,
                            Name = "John F Kennedy Airport",
                            Code = "JFK",
                            City = new()
                            {
                                Id = 2,
                                Name = "????",
                                State = new()
                                {
                                    Id = 2,
                                    Code = "WA"
                                }
                            }
                        }
                    }
                };

                var airports = new List<AirportMongo>()
                {
                  new()
                  {
                      Id = 1,
                      Name = "Los angeles airport",
                      Code = "LAX",
                      City = new()
                      {
                         Id = 1,
                         Name = "Los Angeles",
                         State = new()
                         {
                             Id = 1,
                             Code = "CA"
                         }
                      }
                  },
                  new()
                  {
                      Id = 2,
                      Name = "JFK International Airport",
                      Code = "JFK",
                      City = new()
                      {
                         Id = 2,
                         Name = "New York",
                         State = new()
                         {
                             Id = 2,
                             Code = "NY"
                         }
                      }
                  }

                };

                var users = new List<UserMongo>()
                {
                    new()
                    {
                        Id = 1,
                        Email = "admin@example.com",
                        Password = "AQAAAAIAAYagAAAAEJvAdN3g69LF6cuKWK/xIHyUyz1qtNoVCMgKIlSd5oTPwk+7/A+qEAcxQJ2B+FvghQ==",
                        Role = UserRole.Admin,
                    }
                };

                var booking = new BookingMongo
                {
                    Id = 1,
                    ConfirmationNumber = "ABC123456",
                    User = new UserSnapshot
                    {
                        Id = 7,
                        Email = "user@example.com",
                        Password = "fdsfdggdf",
                        Role = UserRole.Customer
                    },
                    Tickets = new List<TicketEmbedded>
    {
        new TicketEmbedded
        {
            Id = 101,
            Price = 299.99m,
            TicketNumber = "TICKET123",
            TicketsBookingId = 1,
            Passenger = new PassengerEmbedded
            {
                Id = 7,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            },
            FlightClass = new FlightClassSnapshot
            {
                Id = 1,
                Name = FlightClassName.EconomyClass,
                PriceMultiplier = 1.0m
            },
            // Including the FlightSnapshot
            Flight = new FlightSnapShot
            {
                Id = 501,
                FlightCode = "FLIGHT123",
                DepartureTime = DateTime.UtcNow.AddHours(2),
                CompletionTime = DateTime.UtcNow.AddHours(5),
                TravelTime = 180,
                Kilometers = 1500,
                Price = 299.99m,
                EconomyClassSeatsAvailable = 50,
                BusinessClassSeatsAvailable = 10,
                FirstClassSeatsAvailable = 5,
                ArrivalPort = new AirportSnapshot
                {
                    Id = 201,
                    Name = "JFK International Airport",
                    Code = "JFK",
                    City = new CitySnapshot
                    {
                        Id = 301,
                        Name = "New York",
                        State = new StateSnapshot
                        {
                            Id = 401,
                            Code = "NY"
                        }
                    }
                },
                DeparturePort = new AirportSnapshot
                {
                    Id = 202,
                    Name = "Los Angeles International Airport",
                    Code = "LAX",
                    City = new CitySnapshot
                    {
                        Id = 302,
                        Name = "Los Angeles",
                        State = new StateSnapshot
                        {
                            Id = 402,
                            Code = "CA"
                        }
                    }
                },
                FlightsAirline = new AirlineSnapshot
                {
                    Id = 601,
                    Name = "Awesome Airline"
                },
                FlightsAirplane = new AirplaneSnapshot
                {
                    Id = 701,
                    Name = "Boeing 737",
                    EconomyClassSeats = 150,
                    BusinessClassSeats = 30,
                    FirstClassSeats = 10
                }
            }
        }
    }
                };

                _context.Airlines.AddRange(airlines);
                _context.Bookings.Add(booking);
                _context.Users.AddRange(users);
                _context.Airplanes.AddRange(airplanes);
                _context.Flights.AddRange(flights);
                _context.Airports.AddRange(airports);
                _context.SaveChanges();
            }
        }
    }
}
