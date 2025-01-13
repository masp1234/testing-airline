using AutoMapper;
using backend.Config;
using backend.Dtos;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace backend.Tests.Integration
{
    public class BookingServiceIntegrationTests: IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture _dbFixture;
        private readonly IBookingService _bookingService;
        private readonly FlightClass _flightClass = new()
        {
            Id = 1,
            Name = FlightClassName.EconomyClass
        };
        private readonly Airplane _existingAirplane = new()
        {
            Id = 1,
            Name = "Boeing 737",
            AirplanesAirlineId = 1,
            EconomyClassSeats = 140,
            BusinessClassSeats = 40,
            FirstClassSeats = 5,
        };

        private readonly Airline _existingAirline = new()
        {
            Id = 1,
            Name = "Delta Airlines"
        };
        private readonly Airport _existingAirport1 = new ()
        {
            Id = 1,
            Name = "Los Angeles International Airport",
            Code = "LAX",
            CityId = null,
            City = null

        };
        private readonly Airport _existingAirport2 = new ()
        {
            Id = 2,
            Name = "John F. Kennedy International Airport",
            Code = "JFK",
            CityId = null,
            City = null

        };


        private readonly Flight _existingFlight = new()
        {
            Id = 1,
            FlightCode = "DL100",
            DepartureTime = new DateTime(2025, 1, 24, 8, 0, 0),
            CompletionTime = new DateTime(2025, 1, 24, 12, 0, 0),
            TravelTime = 360,
            Kilometers = 450,
            Price = 199,
            IdempotencyKey = "IdempotencyKeyfmefwe",
            EconomyClassSeatsAvailable = 140,
            FirstClassSeatsAvailable = 5,
            BusinessClassSeatsAvailable = 40,
            ArrivalPort = 2 ,
            DeparturePort = 1,
            FlightsAirlineId= 1 ,
            FlightsAirplaneId = 1,
        };

        private readonly User _existingUser1 = new()
        {
            Id = 1, 
            Email = "test1@example.com",
            Password = "123123"
        };
        private readonly User _existingUser2 = new()
        {
            Id = 2, 
            Email = "test2@example.com",
            Password = "123123"
        };

        private readonly Booking _existingBooking1 = new()
        {

            Id = 1,
            ConfirmationNumber = "ABC1",
            UserId = 1
            
        };
        private readonly Booking _existingBooking2 = new()
        {

            Id = 2,
            ConfirmationNumber = "ABC2",
            UserId = 1
            
        };

        public BookingServiceIntegrationTests(TestDatabaseFixture dbFixture )
        {
            _dbFixture = dbFixture;
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            IFlightRepository flightRepository =new FlightRepository(_dbFixture.DbContext);
            IEmailService emailService = new EmailService();
            IMapper mapper = configuration.CreateMapper();

            IUserService userService = new UserService(new UserRepository(_dbFixture.DbContext),  new PasswordHasher<User>(), mapper );

            Mock<IDistanceApiService> mockDistanceApiService = new();

            IFlightService flightService = new FlightService(flightRepository, mapper, mockDistanceApiService.Object, new AirportRepository(_dbFixture.DbContext), emailService, new AirplaneService(new AirplaneRepository(_dbFixture.DbContext), flightRepository, mapper));

            ITicketAvailabilityChecker ticketAvailabilityChecker = new TicketAvailabilityChecker();

            _bookingService = new BookingService(userService, flightService, emailService, new BookingRepository(_dbFixture.DbContext, mapper), mapper, ticketAvailabilityChecker );


            _dbFixture.ResetDatabase();
            _dbFixture.DbContext.ChangeTracker.Clear();
            _dbFixture.DbContext.Bookings.Add(_existingBooking1);
            _dbFixture.DbContext.Bookings.Add(_existingBooking2);
            _dbFixture.DbContext.Users.Add(_existingUser1);
            _dbFixture.DbContext.Users.Add(_existingUser2);
            _dbFixture.DbContext.Flights.Add(_existingFlight);
             _dbFixture.DbContext.Airports.Add(_existingAirport1);
            _dbFixture.DbContext.Airports.Add(_existingAirport2);
            _dbFixture.DbContext.Airplanes.Add(_existingAirplane);
            _dbFixture.DbContext.Airlines.Add(_existingAirline);
            _dbFixture.DbContext.FlightClasses.Add(_flightClass);
            _dbFixture.DbContext.SaveChanges();
            _dbFixture.DbContext.ChangeTracker.Clear();
        }

        [Fact]
        public async Task GetBookingsByUserEmail_ReturnsBookings_WhenUserExists()
        {
            var Bookings = await _bookingService.GetBookingsByUserEmail(_existingUser1.Email);


            Assert.True(Bookings.IsSucces);
            Assert.NotNull(Bookings.Data);
            Assert.Equal(2, Bookings.Data.Count);
            Assert.Contains(Bookings.Data, b => b.ConfirmationNumber == "ABC1");
            Assert.Contains(Bookings.Data, b => b.ConfirmationNumber == "ABC2");
        }

        [Fact]
        public async Task GetBookingsByUserEmail_ReturnsEmptyList_WhenNoBookingsExist()
        {
            var Bookings = await _bookingService.GetBookingsByUserEmail(_existingUser2.Email);


            Assert.True(Bookings.IsSucces);
            Assert.NotNull(Bookings.Data);
            Assert.Empty(Bookings.Data);
        }

        [Fact]
        public async Task GetBookingsByUserEmail_ReturnsFailure_WhenUserNotFound()
        {
            var email = "test1000@example.com";
            

            var Bookings = await _bookingService.GetBookingsByUserEmail(email);

            Assert.False(Bookings.IsSucces);
            Assert.Equal($"No user found with the email {email}", Bookings.Message);
            Assert.Null(Bookings.Data);
        }

        [Fact]
        public async Task CreateBooking_ReturnsFailure_WhenUserNotFound()
        {
            var notExistingUser = new UserResponse
            {
                Id = 1999, 
                Email = "nonexistent@example.com"
            };
            var bookingRequest = new BookingCreationRequest
            {
                Email = notExistingUser.Email,
                Tickets =
                [
                    new TicketCreationRequest { FlightId = 1, FlightClassId = 1 }
                ]
            };

            var booking = await _bookingService.CreateBooking(bookingRequest);

            Assert.False(booking.IsSucces);
            Assert.Equal($"No user found with the email: {bookingRequest.Email}.", booking.Message);
        }

        [Fact]
        public async Task CreateBooking_ReturnsFailure_WhenFlightNotFound()
        {
            var bookingRequest = new BookingCreationRequest
            {
                Email = _existingUser1.Email,
                Tickets =
                [
                    new TicketCreationRequest { FlightId = 999, FlightClassId = 1 }
                ]
            };

            var booking = await _bookingService.CreateBooking(bookingRequest);

            Assert.False(booking.IsSucces);
            Assert.Equal("No flight found with the id: 999.", booking.Message);
        }

        [Fact]
        public async Task CreateBooking_ReturnsFailure_WhenFlightClassNotFound()
        {
            var bookingRequest = new BookingCreationRequest
            {
                Email = _existingUser1.Email,
                Tickets =
                [
                    new TicketCreationRequest { FlightId = _existingFlight.Id, FlightClassId = 999 }
                ]
            };

            var booking = await _bookingService.CreateBooking(bookingRequest);

            Assert.False(booking.IsSucces);
            Assert.Equal("No flight class found with the id: 999.", booking.Message);
        }

        [Fact]
        public async Task CreateBooking_ReturnsFailure_WhenTicketsUnavailable()
        {
            var  unavailableTicketsFlight = new Flight
            {
                Id = 100,
                FlightCode = "DL100",
                DepartureTime = new DateTime(2025, 6, 24, 8, 0, 0),
                CompletionTime = new DateTime(2025, 6, 24, 12, 0, 0),
                TravelTime = 360,
                Kilometers = 450,
                Price = 199,
                IdempotencyKey = "UnavailableTicketsFlight",
                EconomyClassSeatsAvailable = 0,
                FirstClassSeatsAvailable = 0,
                BusinessClassSeatsAvailable = 0,
                ArrivalPort = 2 ,
                DeparturePort = 1,
                FlightsAirlineId= 1 ,
                FlightsAirplaneId = 1,
            };
            _dbFixture.DbContext.Flights.Add(unavailableTicketsFlight);
            _dbFixture.DbContext.SaveChanges();

            var bookingRequest = new BookingCreationRequest
            {
                Email = _existingUser1.Email,
                Tickets =
                [
                    new TicketCreationRequest { FlightId = unavailableTicketsFlight.Id, FlightClassId = 1 }
                ]
            };

            var booking = await _bookingService.CreateBooking(bookingRequest);

            Assert.False(booking.IsSucces);
            Assert.Equal("Some of the tickets requested are unavailable.", booking.Message);
        }

        [Fact]
        public async Task CreateBooking_CreatesSuccessfully_WhenDataIsValid()
        {

                var bookingRequest = new BookingCreationRequest
                {
                    Email = _existingUser1.Email,
                    Tickets =
                    [
                        new TicketCreationRequest
                        {
                            FlightId = _existingFlight.Id,
                            FlightClassId = 1,
                            Passenger = new PassengerCreationRequest
                            {
                                FirstName = "test",
                                LastName = "testsen",
                                Email = "test@testsen.dk"
                            }
                        }
                    ]
                };

            var result = await _bookingService.CreateBooking(bookingRequest);

            Assert.True(result.IsSucces);
            Assert.NotNull(result.Data);
            Assert.Equal("The booking was created successfully.", result.Message);
        }


    }
}
