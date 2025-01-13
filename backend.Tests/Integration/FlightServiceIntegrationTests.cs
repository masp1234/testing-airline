using AutoMapper;
using backend.Config;
using backend.Dtos;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace backend.Tests.Integration
{
    public class FlightServiceIntegrationTests: IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture _dbFixture;
        private readonly IFlightService _flightService;
        private readonly IFlightRepository _flightRepository;
        private readonly Mock<IDistanceApiService> _mockDistanceApiService;

        private readonly Mock<IAirplaneService> _mockAirplaneService;
        private readonly User _existingUser1 = new()
        {
            Id = 1, 
            Email = "test1@example.com",
            Password = "123123"
        };
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
        private readonly Flight _existingFlight2 = new()
        {
            Id = 2,
            FlightCode = "DL101",
            DepartureTime = new DateTime(2025, 4, 26, 8, 0, 0),
            CompletionTime = new DateTime(2025, 4, 26, 12, 0, 0),
            TravelTime = 360,
            Kilometers = 450,
            Price = 199,
            IdempotencyKey = "IdempotencyKeyfmefsdsdcwe",
            EconomyClassSeatsAvailable = 140,
            FirstClassSeatsAvailable = 5,
            BusinessClassSeatsAvailable = 40,
            ArrivalPort = 2 ,
            DeparturePort = 1,
            FlightsAirlineId= 1 ,
            FlightsAirplaneId = 1,
        };
        private readonly Booking _existingBooking = new()
        {
            Id = 1,
            ConfirmationNumber = "ConfirmationNumber2323",
            UserId = 1
        };
        private readonly Ticket _ticket1 = new() 
        {
            Id = 1,
            TicketsBookingId = 1,
            FlightId = 2,
            FlightClassId = 1,
            TicketNumber = "TicketNumber1",
            Passenger = new Passenger { Id = 1, FirstName = "John", LastName= "Doe", Email= "testemail@test.com" },
                             
            };
            private readonly Ticket _ticket2 =new() 
            {
                Id = 2,
                TicketsBookingId = 1,
                FlightId = 2,
                FlightClassId = 1,
                TicketNumber = "TicketNumber2",
                Passenger = new Passenger { Id = 2, FirstName = "Lars", LastName= "Doe", Email= "testemail2@test.com"}
        };
                             


        public FlightServiceIntegrationTests(TestDatabaseFixture dbFixture )
        {
            _dbFixture = dbFixture;
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });


            IMapper mapper = configuration.CreateMapper();
            IFlightRepository flightRepository =new FlightRepository(_dbFixture.DbContext);
            
            _mockDistanceApiService = new Mock<IDistanceApiService>();
            
            IEmailService emailService = new EmailService();
            
            IAirplaneService airplaneService = new AirplaneService(new AirplaneRepository(_dbFixture.DbContext), flightRepository, mapper);

            _flightService = new FlightService(flightRepository, mapper , _mockDistanceApiService.Object, new AirportRepository(_dbFixture.DbContext), emailService, airplaneService);


            _dbFixture.ResetDatabase();

            _dbFixture.DbContext.ChangeTracker.Clear();
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
        public async Task GetAllflights_ShouldReturnListOfflights()
        {
            var flights = await _flightService.GetAllFlights();
            Assert.Single(flights);
        }

        [Fact]
        public async Task GetAllflights_ShouldReturnEmptyList_When_NoFlightsFound()
        {
            _dbFixture.DbContext.Flights.Remove(_existingFlight);
            await _dbFixture.DbContext.SaveChangesAsync();

            var flights = await _flightService.GetAllFlights();

            Assert.Empty(flights);
        }

        [Fact]
        public async Task GetFlightById_ShouldReturnFlight_When_FlightExists()
        {
            var flight = await _flightService.GetFlightById(_existingFlight.Id);
            Assert.NotNull(flight);
            Assert.True(flight.Id == _existingFlight.Id);
        }

        [Fact]
        public async Task GetFlightById_ShouldReturnNull_When_FlightDoesNotExists()
        {
            var flight = await _flightService.GetFlightById(100);
            Assert.Null(flight);
        }
        
        [Fact]
        public async Task GetFlightWithRelationshipsById_ShouldReturnFlight_When_FlightExists()
        {
            var flight = await _flightService.GetFlightWithRelationshipsById(_existingFlight.Id);
            Assert.NotNull(flight);
            Assert.True(flight.Id == _existingFlight.Id);
        }

        [Fact]
        public async Task GetFlightWithRelationshipsById_ShouldReturnNull_When_FlightDoesNotExists()
        {
            var flight = await _flightService.GetFlightWithRelationshipsById(100);
            Assert.Null(flight);
        }

        [Fact]
        public async Task CreateFlight_ReturnsExistingFlight_WhenFlightAlreadyCreated()
        {
            var flightCreationRequest = new FlightCreationRequest
            {
                IdempotencyKey = _existingFlight.IdempotencyKey,
                AirplaneId = 1,
                AirlineId = 1,
                DepartureAirportId = 1,
                ArrivalAirportId = 2,
                DepartureDateTime = DateTime.UtcNow.AddDays(100)
            };

            var flight = await _flightService.CreateFlight(flightCreationRequest);

            Assert.NotNull(flight);
            Assert.Equal(_existingFlight.IdempotencyKey, flight.IdempotencyKey);
            Assert.Equal(_existingFlight.FlightsAirlineId, flight.FlightsAirlineId);
            Assert.Equal(_existingFlight.FlightsAirplaneId, flight.FlightsAirplaneId);
            Assert.Equal(_existingFlight.DepartureTime, flight.DepartureTime);
            Assert.Equal(_existingFlight.DeparturePort, flight.DeparturePort);
            Assert.Equal(_existingFlight.ArrivalPort, flight.ArrivalPort);
        }

        [Theory]
        [InlineData(1, 99)]
        [InlineData(99, 1)]
        [InlineData(99, 99)]
        public async Task CreateFlight_ThrowsException_WhenEitherAirportDoesNotExist(int departureAirportId, int arrivalAirportId)
        {
            var flightCreationRequest = new FlightCreationRequest
            {
                IdempotencyKey = "SDSDSA!23",
                AirplaneId = 1,
                AirlineId = 1,
                DepartureAirportId = departureAirportId,
                ArrivalAirportId = arrivalAirportId,
                DepartureDateTime = DateTime.UtcNow.AddDays(100)
            };

            var exception = await Assert.ThrowsAsync<InvalidDataException>(async() => await _flightService.CreateFlight(flightCreationRequest));
            Assert.Contains("Could not find origin airport or arrival airport", exception.Message);
        }

        [Fact]
        public async Task CreateFlight_ThrowsException_WhenAirplaneDoesNotExist()
        {
            var flightCreationRequest = new FlightCreationRequest
            {
                IdempotencyKey = "SDSDSA!2SD3",
                AirplaneId = 999,
                AirlineId = 1,
                DepartureAirportId = 1,
                ArrivalAirportId = 2,
                DepartureDateTime = DateTime.UtcNow.AddDays(100)
            };

            var exception = await Assert.ThrowsAsync<InvalidDataException>(async () => await _flightService.CreateFlight(flightCreationRequest));
            Assert.Contains("The chosen airplane could not be found", exception.Message);
        }

        [Fact]
        public async Task CreateFlight_CreatesFlightSuccessfully_WhenDataIsValid()
        {
            var flightCreationRequest = new FlightCreationRequest
            {
                IdempotencyKey = "IdempotencyKeyToTest",
                AirplaneId = 1,
                AirlineId = 1,
                DepartureAirportId = 1,
                ArrivalAirportId = 2,
                DepartureDateTime = DateTime.UtcNow.AddDays(100)
            };

            _mockDistanceApiService.Setup(service => service.GetDistanceData(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(500000);

            var flight = await _flightService.CreateFlight(flightCreationRequest);

            Assert.NotNull(flight);
            Assert.Equal(flightCreationRequest.IdempotencyKey, flight.IdempotencyKey);
            Assert.Equal(flightCreationRequest.AirlineId, flight.FlightsAirlineId);
            Assert.Equal(flightCreationRequest.AirplaneId, flight.FlightsAirplaneId);
            Assert.Equal(flightCreationRequest.DepartureDateTime, flight.DepartureTime);
            Assert.Equal(flightCreationRequest.DepartureAirportId, flight.DeparturePort);
            Assert.Equal(flightCreationRequest.ArrivalAirportId, flight.ArrivalPort);
            
        }

        [Fact]
        public async Task CreateFlight_ThrowsException_WhenDistanceDataIsNull()
        {
            var flightCreationRequest = new FlightCreationRequest
            {
                IdempotencyKey = "IdempotencyKeyToTest123",
                AirplaneId = 1,
                AirlineId = 1,
                DepartureAirportId = 1,
                ArrivalAirportId = 2,
                DepartureDateTime = DateTime.UtcNow.AddDays(100)
            };

            _mockDistanceApiService
                .Setup(service => service.GetDistanceData(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((double?)null);

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _flightService.CreateFlight(flightCreationRequest));
            Assert.Contains("Missing distance when trying to create a flight", exception.Message);
        }

        [Theory]
        [InlineData(11, 30)]
        [InlineData(11, 45)]
        [InlineData(11, 59)]
        public async void CreateFlight__Returns_Exception_WhenOverlappingFlight(int hour,  int minutes)
        {

            var flightCreationRequest = new FlightCreationRequest
                {
                    IdempotencyKey = "IdempotencyKeyToTest",
                    AirplaneId = 1,
                    AirlineId = 1,
                    DepartureAirportId = 1,
                    ArrivalAirportId = 2,
                    DepartureDateTime = new DateTime(2025, 1, 24, hour, minutes, 0),
                };


            _mockDistanceApiService.Setup(service => service.GetDistanceData(_existingAirport1.Name, _existingAirport2.Name)).ReturnsAsync(500000);

            await Assert.ThrowsAsync<Exception>(() => _flightService.CreateFlight(flightCreationRequest));
          
        }

        [Fact]
        public async Task CreateFlight_CreatesFlightSuccessfully_WhenNoMoreOverlappingFlight()
        {
            var flightCreationRequest = new FlightCreationRequest
            {
                IdempotencyKey = "WhenNoMoreOverlappingFlightIdempotencyKeyToTest",
                AirplaneId = 1,
                AirlineId = 1,
                DepartureAirportId = 1,
                ArrivalAirportId = 2,
                DepartureDateTime = new DateTime(2025, 1, 24, 12, 1, 0)
            };


            _mockDistanceApiService.Setup(service => service.GetDistanceData(_existingAirport1.Name, _existingAirport2.Name)).ReturnsAsync(500000);

            var flight = await _flightService.CreateFlight(flightCreationRequest);

            Assert.NotNull(flight);
            Assert.Equal(flightCreationRequest.IdempotencyKey, flight.IdempotencyKey);
            Assert.Equal(flightCreationRequest.AirlineId, flight.FlightsAirlineId);
            Assert.Equal(flightCreationRequest.AirplaneId, flight.FlightsAirplaneId);
            Assert.Equal(flightCreationRequest.DepartureDateTime, flight.DepartureTime);
            Assert.Equal(flightCreationRequest.DepartureAirportId, flight.DeparturePort);
            Assert.Equal(flightCreationRequest.ArrivalAirportId, flight.ArrivalPort);
            
        }

        [Fact]
        public async Task CreateFlight_CalculatesCorrectPrice_WhenDataIsValid()
        {
            var flightCreationRequest = new FlightCreationRequest
            {
                IdempotencyKey = "UniqueKeyForPriceTest",
                AirplaneId = 1,
                AirlineId = 1,
                DepartureAirportId = 1,
                ArrivalAirportId = 2,
                DepartureDateTime = DateTime.UtcNow.AddDays(100)
            };

            _mockDistanceApiService.Setup(service => service.GetDistanceData(_existingAirport1.Name, _existingAirport2.Name)).ReturnsAsync(500000); // 500 kilometers

            var createdFlight = await _flightService.CreateFlight(flightCreationRequest);

            Assert.NotNull(createdFlight);
            Assert.Equal(500, createdFlight.Kilometers); // 500 km
            Assert.Equal(35m, createdFlight.Price); // 500 km * 0.07 = 35.00
        }

        [Fact]
        public async Task UpdateFlight_ShouldRetunTrue_WhenFlightUpdateSuccessfully()
        {
            var updateFlightRequest = new UpdateFlightRequest
            {
                DepartureDateTime = new DateTime(2025, 1, 25, 8, 0, 0),
            };

            var updatedFlight = await _flightService.UpdateFlight(updateFlightRequest, _existingFlight);

            Assert.True(updatedFlight);
        }

        [Fact]
        public async Task UpdateFlight_ReturnsFalse_WhenUpdateFails()
        {
            var updateFlightRequest = new UpdateFlightRequest
            {
                DepartureDateTime = new DateTime(2025, 1, 25, 8, 0, 0),
            };

            var flight = new Flight
            {
                Id = 100,
                TravelTime = 120,
                DepartureTime = DateTime.UtcNow.AddDays(1)
            };

            
            var updatedFlight = await _flightService.UpdateFlight(updateFlightRequest, flight);

            Assert.False(updatedFlight);
            
        }

        [Fact]
        public async Task UpdateFlight_CalculatesCompletionTimeCorrectly()
        {
            var updateFlightRequest = new UpdateFlightRequest
            {
                DepartureDateTime = new DateTime(2025, 1, 25, 8, 0, 0),
            };


            await _flightService.UpdateFlight(updateFlightRequest, _existingFlight);
            var updatedFlight = await _flightService.GetFlightById(_existingFlight.Id);

            int simulatedPreparationTimeInMinutes = 120;

            var expectedCompletionTime = updateFlightRequest.DepartureDateTime.AddMinutes(_existingFlight.TravelTime + simulatedPreparationTimeInMinutes);
            Assert.Equal(expectedCompletionTime, updatedFlight?.CompletionTime);
        }

        [Theory]
        [InlineData(11, 30)]
        [InlineData(11, 45)]
        [InlineData(11, 59)]
        public async void UpdateFlight__Returns_Exception_WhenOverlappingFlight(int hour,  int minutes)
        {
            _dbFixture.DbContext.Flights.Add(_existingFlight2);
            await _dbFixture.DbContext.SaveChangesAsync();

            var updateFlightRequest = new UpdateFlightRequest
                {
                    DepartureDateTime = new DateTime(2025, 4, 26, hour, minutes, 0),
                };

            await Assert.ThrowsAsync<Exception>(() => _flightService.UpdateFlight(updateFlightRequest, _existingFlight));
          
        }


        [Fact]
        public async Task CancelFlight_CancelsFlightSuccessfully_WhenFlightExists()
        {

            await _flightService.CancelFlight(_existingFlight.Id);

            var deletedFlight = await _flightService.GetFlightById(_existingFlight.Id);
            Assert.Null(deletedFlight);
        }

        [Fact]
        public async Task CancelFlight_ThrowsException_WhenFlightDoesNotExist()
        {
            int nonExistentFlightId = 999;

            await Assert.ThrowsAsync<Exception>(() => _flightService.CancelFlight(nonExistentFlightId));
        }

        [Fact]
        public async Task GetFlightsByDepartureDestinationAndDepartureDate_ReturnsFlights_WhenTheyExists()
        {
            DateOnly departureDate = DateOnly.FromDateTime(_existingFlight.DepartureTime);
            var flights = await _flightService.GetFlightsByDepartureDestinationAndDepartureDate(_existingAirport1.Id, _existingAirport2.Id, departureDate);

            Assert.True(flights.Count == 1);
        }

        [Fact]
        public async Task GetFlightsByDepartureDestinationAndDepartureDate_ReturnsEmptyList_WhenNoMatchingFlights()
        {
            int nonExistingId = 100;
            DateOnly departureDate = DateOnly.FromDateTime(_existingFlight.DepartureTime);
            var flights = await _flightService.GetFlightsByDepartureDestinationAndDepartureDate(nonExistingId, _existingAirport2.Id, departureDate);

            Assert.Empty(flights);
        }
        public async Task CancelFlight_ShouldDeleteFlightAndRelatedEntities()
        {

            _dbFixture.DbContext.Users.Add(_existingUser1);
            _dbFixture.DbContext.Flights.Add(_existingFlight2);
            _dbFixture.DbContext.Bookings.Add(_existingBooking);
            _dbFixture.DbContext.Tickets.Add(_ticket1);
            _dbFixture.DbContext.Tickets.Add(_ticket2);
            _dbFixture.DbContext.SaveChanges();

            await _flightService.CancelFlight(_existingFlight2.Id);

            var deletedFlight = await _flightService.GetFlightById(_existingFlight2.Id);

            var deletedTickets = await _flightRepository.GetTicketsByFlightId(_existingFlight2.Id);
            var deletedPassenger1 = await _dbFixture.DbContext.Passengers.FindAsync(1);
            var deletedPassenger2 =await _dbFixture.DbContext.Passengers.FindAsync(2);

            Assert.Null(deletedFlight);
            Assert.Empty(deletedTickets);
            Assert.Null(deletedPassenger1);
            Assert.Null(deletedPassenger2);
        }

        [Fact]
        public async Task GetFlightClassById_ShouldReturnFlightClass_WhenIdExists()
        {

            var flightClass = await _flightService.GetFlightClassById(1);

            Assert.NotNull(flightClass);
            Assert.Equal(FlightClassName.EconomyClass, flightClass.Name);
        }

        [Fact]
        public async Task GetFlightClassById_ShouldReturnNull_WhenIdDoesNotExist()
        {
            var flightClass = await _flightService.GetFlightClassById(1999);

            Assert.Null(flightClass);
        }

    }

}   
