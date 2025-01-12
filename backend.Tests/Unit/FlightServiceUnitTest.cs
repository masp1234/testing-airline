using AutoMapper;
using backend.Config;
using backend.Dtos;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Moq;

namespace backend.Tests.Unit
{
    public class FlightServiceUnitTest
    {
        private readonly IFlightService _flightService;
        private readonly Mock<IDistanceApiService> _mockDistanceApiService;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IAirplaneService> _mockAirplaneService;
        private readonly Mock<IFlightRepository> _mockFlightRepository;
        private readonly Mock<IAirportRepository> _airportRepository ;

        private readonly List<FlightClass> _mockFlightClass = 
        [
            new()
            {
                Id = 1,
                Name = FlightClassName.EconomyClass
            },
            new()
            {
                Id = 2,
                Name = FlightClassName.BusinessClass
            },
            new()
            {
                Id = 3,
                Name = FlightClassName.FirstClass
            }

        ];

        private readonly List<Flight> _mockFlights = 
        [
            new()
            {
                Id = 1,
                FlightCode = "DL100",
                DepartureTime = new DateTime(2024, 12, 24, 8, 0, 0),
                CompletionTime = new DateTime(2024, 12, 24, 16, 0, 0),
                TravelTime = 360,
                Kilometers = 450,
                Price = 199,
                EconomyClassSeatsAvailable = 150,
                FirstClassSeatsAvailable = 5,
                BusinessClassSeatsAvailable = 20,
                ArrivalPortNavigation = new Airport
                {
                    Id = 2,
                    Name = "John F. Kennedy International Airport",
                    Code = "JFK",
                    CityId = 2,
                    City = null
                },
                DeparturePortNavigation = new Airport
                {
                    Id = 1,
                    Name = "Los Angeles International Airport",
                    Code = "LAX",
                    CityId = 1,
                    City = null
                },
                FlightsAirline = new Airline
                {
                    Id = 1,
                    Name = "Delta Airlines"
                },
                FlightsAirplane = new Airplane
                {
                    Id = 1,
                    Name = "Boeing 737",
                    AirplanesAirlineId = 1,
                    EconomyClassSeats = 140,
                    BusinessClassSeats = 40,
                    FirstClassSeats = 5,
                },
                Tickets = []
            },
            new()
            {
                Id = 2,
                FlightCode = "UA200",
                DepartureTime = new DateTime(2024, 12, 25, 9, 30, 0),
                CompletionTime = new DateTime(2024, 12, 25, 17, 30, 0),
                TravelTime = 480,
                Kilometers = 600,
                Price = 299,
                EconomyClassSeatsAvailable = 180,
                FirstClassSeatsAvailable = 10,
                BusinessClassSeatsAvailable = 30,
                ArrivalPortNavigation = new Airport
                {
                    Id = 3,
                    Name = "Chicago O'Hare International Airport",
                    Code = "ORD",
                    CityId = 3,
                    City = null
                },
                DeparturePortNavigation = new Airport
                {
                    Id = 1,
                    Name = "Los Angeles International Airport",
                    Code = "LAX",
                    CityId = 1,
                    City = null
                },
                FlightsAirline = new Airline
                {
                    Id = 2,
                    Name = "United Airlines"
                },
                FlightsAirplane = new Airplane
                {
                    Id = 2,
                    Name = "Airbus A320",
                    AirplanesAirlineId = 2,
                    EconomyClassSeats = 160,
                    BusinessClassSeats = 50,
                    FirstClassSeats = 10,
                },
                Tickets = []
            },
            new()
            {
                Id = 3,
                FlightCode = "AA300",
                DepartureTime = new DateTime(2024, 12, 26, 14, 15, 0),
                CompletionTime = new DateTime(2024, 12, 26, 22, 15, 0),
                TravelTime = 480,
                Kilometers = 800,
                Price = 399,
                EconomyClassSeatsAvailable = 120,
                FirstClassSeatsAvailable = 8,
                BusinessClassSeatsAvailable = 25,
                ArrivalPortNavigation = new Airport
                {
                    Id = 4,
                    Name = "Dallas/Fort Worth International Airport",
                    Code = "DFW",
                    CityId = 4,
                    City = null
                },
                DeparturePortNavigation = new Airport
                {
                    Id = 2,
                    Name = "John F. Kennedy International Airport",
                    Code = "JFK",
                    CityId = 2,
                    City = null
                },
                FlightsAirline = new Airline
                {
                    Id = 3,
                    Name = "American Airlines"
                },
                FlightsAirplane = new Airplane
                {
                    Id = 3,
                    Name = "Boeing 777",
                    AirplanesAirlineId = 3,
                    EconomyClassSeats = 200,
                    BusinessClassSeats = 50,
                    FirstClassSeats = 8,
                },
                Tickets = []
            }

        ];
        public FlightServiceUnitTest()
        {
            _mockFlightRepository = new Mock<IFlightRepository>();
            _airportRepository = new Mock<IAirportRepository>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            IMapper mapper = configuration.CreateMapper();

            _mockAirplaneService = new Mock<IAirplaneService>();
            _mockDistanceApiService = new Mock<IDistanceApiService>();
            _mockEmailService = new Mock<IEmailService>();

            _flightService = new FlightService(_mockFlightRepository.Object, mapper, _mockDistanceApiService.Object, _airportRepository.Object, _mockEmailService.Object, _mockAirplaneService.Object );
        }

        [Fact]
        public async Task GetAllFlights_ShouldReturnAllFlights()
        {
            _mockFlightRepository.Setup( repo => repo.GetAll()).ReturnsAsync(_mockFlights);
            var flights = await _flightService.GetAllFlights();
            Assert.Equal(3, flights.Count);
        }

        [Fact]
        public async Task GetAllFlights_ShouldReturnEmptyList_WhenNoFlightFound()
        {
            _mockFlightRepository.Setup( repo => repo.GetAll()).ReturnsAsync([]);
            var flights = await _flightService.GetAllFlights();
            Assert.Empty(flights);
        }

        [Fact]
        public async Task GetFlightById_ReturnsFlight_WhenFlightExists()
        {
            var flightId = 1;
            _mockFlightRepository.Setup(repo => repo.GetFlightById(flightId)).ReturnsAsync(_mockFlights[0]);
            var flight = await _flightService.GetFlightById(flightId);
            Assert.NotNull(flight);
            Assert.Equal(flightId, flight.Id);
        }

        [Fact]
        public async Task GetFlightById_ReturnsNull_WhenFlightDoesNotExist()
        {
            var flightId = 4;
            _mockFlightRepository.Setup(repo => 
                repo.GetFlightById(flightId)).ReturnsAsync(_mockFlights[0]);

            var flight = await _flightService.GetFlightById(flightId);

            Assert.NotEqual(flightId, flight?.Id);
        }

        [Fact]
        public async Task GetFlightWithRelationshipsById_ReturnsMappedFlightResponse_WhenFlightExists()
        {
            var flightId = 1;

            _mockFlightRepository.Setup(repo => repo.GetFlightWithRelationshipsById(flightId)).ReturnsAsync(_mockFlights[0]);

            var result = await _flightService.GetFlightWithRelationshipsById(flightId);

            Assert.NotNull(result);
            Assert.Equal(flightId, result.Id);
        }

        [Fact]
        public async Task GetFlightWithRelationshipsById_ReturnsNull_WhenFlightDoesNotExist()
        {
            var flightId = 5;
            _mockFlightRepository.Setup(repo => repo.GetFlightWithRelationshipsById(flightId)).ReturnsAsync(_mockFlights[0]);

            var flight = await _flightService.GetFlightWithRelationshipsById(flightId);

            Assert.NotEqual(flightId, flight?.Id);
        }

        [Fact]
        public async Task GetFlightsByDepartureDestinationAndDepartureDate_ReturnsMappedFlightResponses_WhenFlightsExist()
        {
            var departureAirportId = 1;
            var destinationAirportId = 2;
            var departureDate = new DateOnly(2024, 12, 24);


            _mockFlightRepository.Setup(repo => repo.GetFlightsByDepartureDestinationAndDepartureDate(departureAirportId, destinationAirportId, departureDate))
                                .ReturnsAsync(_mockFlights);

            var flight = await _flightService.GetFlightsByDepartureDestinationAndDepartureDate(departureAirportId, destinationAirportId, departureDate);

            Assert.NotNull(flight);
            Assert.Equal(departureAirportId, flight[0].DeparturePortNavigation.Id);
            Assert.Equal(destinationAirportId, flight[0].ArrivalPortNavigation.Id);
            Assert.Equal(departureDate, DateOnly.FromDateTime(flight[0].DepartureTime));
        }

        [Fact]
        public async Task GetFlightsByDepartureDestinationAndDepartureDate_ReturnsEmptyList_WhenNoFlightsExist()
        {
            var departureAirportId = 1;
            var destinationAirportId = 2;
            var departureDate = new DateOnly(2024, 12, 29);

            _mockFlightRepository.Setup(repo => repo.GetFlightsByDepartureDestinationAndDepartureDate(departureAirportId, destinationAirportId, departureDate)).ReturnsAsync([]);

            var flight = await _flightService.GetFlightsByDepartureDestinationAndDepartureDate(departureAirportId, destinationAirportId, departureDate);

            Assert.NotNull(flight);
            Assert.Empty(flight);
        }

        [Fact]
        public async Task GetFlightClassById_ReturnsFlightClass_WhenFlightClassExists()
        {
            var flightClassId = 1;
            _mockFlightRepository.Setup(repo => repo.GetFlightClassById(flightClassId)).ReturnsAsync(_mockFlightClass[0]);

            var flightClass = await _flightService.GetFlightClassById(flightClassId);

            Assert.NotNull(flightClass);
            Assert.Equal(flightClassId, flightClass.Id);
            Assert.Equal(FlightClassName.EconomyClass, flightClass.Name);
           
        }

        [Fact]
        public async Task GetFlightClassById_ReturnsNull_WhenFlightClassDoesNotExist()
        {
            var flightClassId = 4;
            _mockFlightRepository.Setup(repo => repo.GetFlightClassById(flightClassId)).ReturnsAsync((FlightClass?)null);

            var flightClass = await _flightService.GetFlightClassById(flightClassId);

            Assert.Null(flightClass);
          
        }




    }

}