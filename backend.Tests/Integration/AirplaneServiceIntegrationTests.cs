using AutoMapper;
using backend.Services;
using backend.Config;
using backend.Repositories;
using backend.Models;

namespace backend.Tests.Integration
{
    public class AirplaneServiceIntegrationTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly IAirplaneService _sut;
        private readonly TestDatabaseFixture _dbFixture;
        private readonly Airline _existingAirline = new() { Id = 1, Name = "Test airline" };
        private readonly Airport _existingDepartureAirport = new() { Id = 1, Name = "Departure Airport", Code = "DEP" };
        private readonly Airport _existingArrivalAirport = new() { Id = 2, Name = "Arrival Airport", Code = "ARR" };
        private readonly Airplane _existingAirplane;

        public AirplaneServiceIntegrationTests(TestDatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            IMapper mapper = configuration.CreateMapper();
            var flightRepository = new FlightRepository(_dbFixture.DbContext);

            _sut = new AirplaneService(new AirplaneRepository(_dbFixture.DbContext), flightRepository, mapper);
            
            _dbFixture.ResetDatabase();

            _dbFixture.DbContext.Airlines.Add(_existingAirline);
            _dbFixture.DbContext.Airports.Add(_existingDepartureAirport);
            _dbFixture.DbContext.Airports.Add(_existingArrivalAirport);

            _existingAirplane = new()
            {
                Id = 1,
                Name = "Test Airplane",
                EconomyClassSeats = 100,
                BusinessClassSeats = 40,
                FirstClassSeats = 20,
                AirplanesAirlineId = _existingAirline.Id,
            };

            _dbFixture.DbContext.SaveChanges();
            _dbFixture.DbContext.ChangeTracker.Clear();
        }

        [Fact]
        public async Task GetAllAirplanes_ShouldReturn_Airplanes()
        {
            _dbFixture.DbContext.Airplanes.Add(_existingAirplane);
            await _dbFixture.DbContext.SaveChangesAsync();
            // Act
            var airplanes = await _sut.GetAllAirplanes();

            // Assert
            Assert.NotEmpty(airplanes);
        }

        [Fact]
        public async Task GetAllAirplanes_ShouldReturnEmptyList_WhenNoAirplanesFound()
        {
            var airplanes = await _sut.GetAllAirplanes();

            // Assert
            Assert.Empty(airplanes);
        }

        [Fact]
        public async Task GetAirplaneById_ShouldReturnNull_WhenNoAirplaneFound()
        {
            var airplane = await _sut.GetAirplaneById(_existingAirplane.Id);

            Assert.Null(airplane);

        }

        [Fact]
        public async Task GetAirplaneById_ShouldReturnAirplane_WhenAirplaneExists()
        {
            _dbFixture.DbContext.Airplanes.Add(_existingAirplane);
            await _dbFixture.DbContext.SaveChangesAsync();

            var airplane = await _sut.GetAirplaneById(_existingAirplane.Id);

            Assert.Equal(_existingAirplane, airplane);
        }

        [Fact]
        public async Task GetBookedTimeSlotsByAirplaneId_ShouldReturnBookedTimeSlots_WhenAirplaneIsBooked()
        {
            var flight = new Flight()
            {
                // Arrange
                FlightCode = "TESTCODE",
                DeparturePort = _existingDepartureAirport.Id,
                ArrivalPort = _existingArrivalAirport.Id,
                DepartureTime = DateTime.UtcNow,
                CompletionTime = DateTime.UtcNow.AddHours(4),
                TravelTime = 120,
                Price = 200,
                EconomyClassSeatsAvailable = _existingAirplane.EconomyClassSeats,
                BusinessClassSeatsAvailable = _existingAirplane.BusinessClassSeats,
                FirstClassSeatsAvailable = _existingAirplane.FirstClassSeats,
                FlightsAirlineId = _existingAirline.Id,
                FlightsAirplaneId = _existingAirplane.Id,
                IdempotencyKey = "TESTKEY"
            };
        
            _dbFixture.DbContext.Airplanes.Add(_existingAirplane);
            _dbFixture.DbContext.Flights.Add(flight);
            await _dbFixture.DbContext.SaveChangesAsync();

            // Act
            var bookedTimeSlots = await _sut.GetBookedTimeslotsByAirplaneId(_existingAirplane.Id);

            // Assert
            Assert.True(bookedTimeSlots.Count == 1);
        }

        [Fact]
        public async Task GetBookedTimeSlotsByAirplaneId_ShouldReturnEmptyList_WhenAirplaneIsNotBooked()
        {
            // Arrange
            _dbFixture.DbContext.Airplanes.Add(_existingAirplane);
            await _dbFixture.DbContext.SaveChangesAsync();

            // Act
            var bookedTimeSlots = await _sut.GetBookedTimeslotsByAirplaneId(_existingAirplane.Id);

            // Assert
            Assert.Empty(bookedTimeSlots);
        }
    }


}
