using AutoMapper;
using backend.Config;
using backend.Dtos;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Moq;

namespace backend.Tests.Integration
{
    public class BookingServiceIntegrationTests: IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture _dbFixture;
        private readonly IBookingService _bookingService;
        private readonly Mock<IFlightService> _mockFlightService;
        private readonly Mock<IUserService> _mockUserService ;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<ITicketAvailabilityChecker> _mockTicketAvailabilityChecker;

        private readonly FlightClass _flightClass = new()
        {
            Id = 1,
            Name = FlightClassName.EconomyClass
        }

        ;

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
            _mockFlightService = new Mock<IFlightService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockTicketAvailabilityChecker = new Mock<ITicketAvailabilityChecker>();
            _mockUserService = new Mock<IUserService>();
            _dbFixture = dbFixture;
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });


            IMapper mapper = configuration.CreateMapper();

            _bookingService = new BookingService(_mockUserService.Object, _mockFlightService.Object, _mockEmailService.Object, new BookingRepository(_dbFixture.DbContext, mapper), mapper,  _mockTicketAvailabilityChecker.Object );

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
            var user = new UserResponse
            {
                Id = 1, 
                Email = "test1@example.com"
            };
            _mockUserService.Setup(service => service.GetUserByEmail(user.Email)).ReturnsAsync(user);

            var Bookings = await _bookingService.GetBookingsByUserEmail(user.Email);

            Assert.True(Bookings.IsSucces);
            Assert.NotNull(Bookings.Data);
            Assert.Equal(2, Bookings.Data.Count);
            Assert.Contains(Bookings.Data, b => b.ConfirmationNumber == "ABC1");
            Assert.Contains(Bookings.Data, b => b.ConfirmationNumber == "ABC2");
        }

        [Fact]
        public async Task GetBookingsByUserEmail_ReturnsEmptyList_WhenNoBookingsExist()
        {
            var user = new UserResponse
            {
                Id = 2, 
                Email = "test2@example.com"
            };

            _mockUserService.Setup(service => service.GetUserByEmail(user.Email)).ReturnsAsync(user);

            var Bookings = await _bookingService.GetBookingsByUserEmail(user.Email);

            Assert.True(Bookings.IsSucces);
            Assert.NotNull(Bookings.Data);
            Assert.Empty(Bookings.Data);
        }

        [Fact]
        public async Task GetBookingsByUserEmail_ReturnsFailure_WhenUserNotFound()
        {
            var email = "test1000@example.com";
            

            _mockUserService.Setup(service => service.GetUserByEmail(email)).ReturnsAsync((UserResponse?)null);

            var Bookings = await _bookingService.GetBookingsByUserEmail(email);

            Assert.False(Bookings.IsSucces);
            Assert.Equal($"No user found with the email {email}", Bookings.Message);
            Assert.Null(Bookings.Data);
        }

        [Fact]
        public async Task CreateBooking_ReturnsFailure_WhenUserNotFound()
        {
            var user = new UserResponse
            {
                Id = 1999, 
                Email = "nonexistent@example.com"
            };
            var bookingRequest = new BookingCreationRequest
            {
                Email = user.Email,
                Tickets =
                [
                    new TicketCreationRequest { FlightId = 1, FlightClassId = 1 }
                ]
            };

            _mockUserService.Setup(service => service.GetUserByEmail(user.Email)).ReturnsAsync((UserResponse?) null);

            var booking = await _bookingService.CreateBooking(bookingRequest);

            Assert.False(booking.IsSucces);
            Assert.Equal($"No user found with the email: {bookingRequest.Email}.", booking.Message);
        }

        [Fact]
        public async Task CreateBooking_ReturnsFailure_WhenFlightNotFound()
        {
            var user = new UserResponse
            {
                Id = 1, 
                Email = "test1@example.com"
            };

            _mockUserService.Setup(service => service.GetUserByEmail(user.Email)).ReturnsAsync(user);

            var bookingRequest = new BookingCreationRequest
            {
                Email = _existingUser1.Email,
                Tickets =
                [
                    new TicketCreationRequest { FlightId = 999, FlightClassId = 1 }
                ]
            };

            _mockFlightService.Setup(service => service.GetFlightById(bookingRequest.Tickets[0].FlightId)).ReturnsAsync((Flight?)null);

            var booking = await _bookingService.CreateBooking(bookingRequest);

            Assert.False(booking.IsSucces);
            Assert.Equal("No flight found with the id: 999.", booking.Message);
        }

        [Fact]
        public async Task CreateBooking_ReturnsFailure_WhenFlightClassNotFound()
        {
            var user = new UserResponse
            {
                Id = 1, 
                Email = "test1@example.com"
            };
            var bookingRequest = new BookingCreationRequest
            {
                Email = _existingUser1.Email,
                Tickets =
                [
                    new TicketCreationRequest { FlightId = _existingFlight.Id, FlightClassId = 999 }
                ]
            };
            _mockUserService.Setup(service => service.GetUserByEmail(_existingUser1.Email)).ReturnsAsync(user);

            _mockFlightService.Setup(service => service.GetFlightById(bookingRequest.Tickets[0].FlightId)).ReturnsAsync(_existingFlight);

            _mockFlightService.Setup(service => service.GetFlightClassById(bookingRequest.Tickets[0].FlightClassId)).ReturnsAsync((FlightClass?) null);

            var booking = await _bookingService.CreateBooking(bookingRequest);

            Assert.False(booking.IsSucces);
            Assert.Equal("No flight class found with the id: 999.", booking.Message);
        }

        [Fact]
        public async Task CreateBooking_ReturnsFailure_WhenTicketsUnavailable()
        {
            var user = new UserResponse
            {
                Id = 1, 
                Email = "test1@example.com"
            };
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

            _mockUserService.Setup(service => service.GetUserByEmail(_existingUser1.Email)).ReturnsAsync(user);

            _mockFlightService.Setup(service => service.GetFlightById(bookingRequest.Tickets[0].FlightId)).ReturnsAsync(unavailableTicketsFlight);

            _mockFlightService.Setup(service => service.GetFlightClassById(bookingRequest.Tickets[0].FlightClassId)).ReturnsAsync(_flightClass);

            _mockFlightService.Setup(service => service.GetFlightClassById(bookingRequest.Tickets[0].FlightClassId)).ReturnsAsync(_flightClass);

            _mockTicketAvailabilityChecker.Setup(service => service.CheckTicketAvailability()).Returns(false);

            var booking = await _bookingService.CreateBooking(bookingRequest);

            Assert.False(booking.IsSucces);
            Assert.Equal("Some of the tickets requested are unavailable.", booking.Message);
        }

        [Fact]
    public async Task CreateBooking_CreatesSuccessfully_WhenDataIsValid()
    {
        var user = new UserResponse
            {
                Id = 1, 
                Email = "test1@example.com"
            };
            
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

            _mockUserService.Setup(service => service.GetUserByEmail(_existingUser1.Email)).ReturnsAsync(user);

            _mockFlightService.Setup(service => service.GetFlightById(bookingRequest.Tickets[0].FlightId)).ReturnsAsync(_existingFlight);

            _mockFlightService.Setup(service => service.GetFlightClassById(bookingRequest.Tickets[0].FlightClassId)).ReturnsAsync(_flightClass);

            _mockFlightService.Setup(service => service.GetFlightClassById(bookingRequest.Tickets[0].FlightClassId)).ReturnsAsync(_flightClass);

            _mockTicketAvailabilityChecker.Setup(service => service.CheckTicketAvailability()).Returns(true);

        var result = await _bookingService.CreateBooking(bookingRequest);

        Assert.True(result.IsSucces);
        Assert.NotNull(result.Data);
        Assert.Equal("The booking was created successfully.", result.Message);
    }


        



    }
}
/*
dotnet test ./backend.Tests/backend.Tests.csproj --filter "FullyQualifiedName~Integration"
*/