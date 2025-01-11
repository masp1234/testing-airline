using AutoMapper;
using backend.Config;
using backend.Dtos;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Moq;

namespace backend.Tests.Integration
{
    public class FlightServiceIntegrationTests: IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture _dbFixture;
        private readonly IFlightService _flightService;
        private readonly Mock<IDistanceApiService> _mockDistanceApiService;
        private readonly IEmailService _emailService;
        private readonly IAirplaneService _airplaneService;
        private readonly Mock<IAirplaneService> _mockAirplaneService;
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


        public FlightServiceIntegrationTests(TestDatabaseFixture dbFixture )
        {
            _dbFixture = dbFixture;
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });


            IMapper mapper = configuration.CreateMapper();
            _mockAirplaneService = new Mock<IAirplaneService>();
            _mockDistanceApiService = new Mock<IDistanceApiService>();

            _flightService = new FlightService(new FlightRepository(_dbFixture.DbContext), mapper , _mockDistanceApiService.Object, new AirportRepository(_dbFixture.DbContext), _emailService, _mockAirplaneService.Object);

            _dbFixture.ResetDatabase();

            _dbFixture.DbContext.ChangeTracker.Clear();
            _dbFixture.DbContext.Flights.Add(_existingFlight);
            _dbFixture.DbContext.Airports.Add(_existingAirport1);
            _dbFixture.DbContext.Airports.Add(_existingAirport2);
            _dbFixture.DbContext.Airplanes.Add(_existingAirplane);
            _dbFixture.DbContext.Airlines.Add(_existingAirline);
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

            var users = await _flightService.GetAllFlights();

            Assert.Empty(users);
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

            _mockAirplaneService.Setup(service => service.GetAirplaneById(flightCreationRequest.AirplaneId))
                                .ReturnsAsync(_existingAirplane);

            _mockDistanceApiService.Setup(service => service.GetDistanceData(It.IsAny<string>(), It.IsAny<string>()))
                           .ReturnsAsync(500000);

            // Act
            var flight = await _flightService.CreateFlight(flightCreationRequest);

            // Assert
            Assert.NotNull(flight);
            Assert.Equal(flightCreationRequest.IdempotencyKey, flight.IdempotencyKey);
            Assert.Equal(flightCreationRequest.AirlineId, flight.FlightsAirlineId);
            Assert.Equal(flightCreationRequest.AirplaneId, flight.FlightsAirplaneId);
            Assert.Equal(flightCreationRequest.DepartureDateTime, flight.DepartureTime);
            Assert.Equal(flightCreationRequest.DepartureAirportId, flight.DeparturePort);
            Assert.Equal(flightCreationRequest.ArrivalAirportId, flight.ArrivalPort);
            
        }

        [Theory]
        [InlineData(11, 30)]
        [InlineData(11, 45)]
        [InlineData(11, 59)]
        public async void CreateFlight__Returns_InvalidOperationException_WhenOverlappingFlight(int hour,  int minutes)
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

            _mockAirplaneService.Setup(service => service.GetAirplaneById(flightCreationRequest.AirplaneId))
                                .ReturnsAsync(_existingAirplane);

            _mockDistanceApiService.Setup(service => service.GetDistanceData(_existingAirport1.Name, _existingAirport2.Name))
                        .ReturnsAsync(500000);
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

            _mockAirplaneService.Setup(service => service.GetAirplaneById(flightCreationRequest.AirplaneId))
                                .ReturnsAsync(_existingAirplane);

            _mockDistanceApiService.Setup(service => service.GetDistanceData(_existingAirport1.Name, _existingAirport2.Name))
                           .ReturnsAsync(500000);

            // Act
            var flight = await _flightService.CreateFlight(flightCreationRequest);

            // Assert
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


            _mockAirplaneService.Setup(service => service.GetAirplaneById(flightCreationRequest.AirplaneId))
                                .ReturnsAsync(_existingAirplane);



            _mockDistanceApiService.Setup(service => service.GetDistanceData(_existingAirport1.Name, _existingAirport2.Name))
                                .ReturnsAsync(500000); // 500 kilometers

            var createdFlight = await _flightService.CreateFlight(flightCreationRequest);

            Assert.NotNull(createdFlight);
            Assert.Equal(500, createdFlight.Kilometers); // 500 km
            Assert.Equal(35m, createdFlight.Price); // 500 km * 0.07 = 35.00
        }






    }

}   
