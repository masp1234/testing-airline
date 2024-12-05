using AutoMapper;
using backend.Dtos;
using backend.Models;
using backend.Repositories;
using backend.Enums;

namespace backend.Services
{
    public class FlightService(
        IFlightRepository flightRepository,
        IMapper mapper,
        IDistanceApiService distanceApiService,
        IAirportRepository airportRepository,
        IEmailService emailService,
        IAirplaneService airplaneService
            ) : IFlightService
    {
        private readonly IFlightRepository _flightRepository = flightRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IDistanceApiService _distanceApiService = distanceApiService;
        private readonly IAirportRepository _airportRepository = airportRepository;
        private readonly IEmailService _emailService = emailService;
        private readonly IAirplaneService _airplaneService = airplaneService;

        public async Task<List<FlightResponse>> GetAllFlights()
        {
            var flights = await _flightRepository.GetAll();
            var mappedFlights = _mapper.Map<List<FlightResponse>>(flights);
            return mappedFlights;
        }

        public async Task<Flight?> GetFlightById(int id)
        {
            var flight = await _flightRepository.GetFlightById(id);
            return flight;
        }

        public async Task<FlightResponse?> GetFlightWithRelationshipsById(int id)
        {
            var flight = await _flightRepository.GetFlightWithRelationshipsById(id);
            var mappedFlight = _mapper.Map<FlightResponse>(flight);
            return mappedFlight;
        }
        public async Task<Flight> CreateFlight(FlightCreationRequest flightCreationRequest)
        {
            Flight? existingFlight = await _flightRepository.GetFlightByIdempotencyKey(flightCreationRequest.IdempotencyKey);
            if (existingFlight != null)
            {
                return existingFlight;
            }
            var airplane = await _airplaneService.GetAirplaneById(flightCreationRequest.AirplaneId);
            if (airplane == null)
            {
                throw new InvalidDataException("The chosen airplane could not be found");
            }
            Flight flight = _mapper.Map<Flight>(flightCreationRequest);
            flight.FlightCode = "123FLIGHTCODE";
            var airports = await _airportRepository.FindByIds(flight.DeparturePort, flight.ArrivalPort);
            Airport? originAirport = airports.Find((airport => airport.Id == flight.DeparturePort));
            Airport? arrivalAirport = airports.Find((airport => airport.Id == flight.ArrivalPort));
            if (originAirport == null || arrivalAirport == null) {
                throw new InvalidDataException("Could not find origin airport or arrival airport.");
            }
            (int distance, int duration) = await GetDistanceAndTravelTime(originAirport.Name, arrivalAirport.Name);
            flight.Kilometers = distance;
            flight.TravelTime = (int)duration;
            // the 120 literal value below is meant to simulate preparation time in minutes between when an airplane lands at an airport, and when it is ready to fly again.
            flight.CompletionTime = flight.DepartureTime.AddMinutes(duration + 120);

            var overLappingFlights = await _flightRepository.GetFlightsByAirplaneIdAndTimeInterval(flight);
            if (overLappingFlights.Count > 0)
            {
                throw new Exception("There was 1 or more overlapping flights.");
            }

            flight.EconomyClassSeatsAvailable = airplane.EconomyClassSeats;
            flight.BusinessClassSeatsAvailable = airplane.BusinessClassSeats;
            flight.FirstClassSeatsAvailable = airplane.FirstClassSeats;
            flight.Price = CalculateFlightPrice(distance);
            Flight createdFlight = await _flightRepository.Create(flight);
            return createdFlight;

        }

        public async Task<List<FlightResponse>> GetFlightsByDepartureDestinationAndDepartureDate(int departureAirportId, int destinationAirportId, DateOnly departureDate)
        {
            var flights = await _flightRepository.GetFlightsByDepartureDestinationAndDepartureDate(departureAirportId, destinationAirportId, departureDate);
            var mappedFlights = _mapper.Map<List<FlightResponse>>(flights);
            return mappedFlights;
        }

        private async Task<(int, int)> GetDistanceAndTravelTime(string origin, string destination)
        {
            double? distance = await _distanceApiService.GetDistanceData(origin, destination);
            if (distance == null)
            {
                throw new ArgumentNullException("Missing distance when trying to create a flight.");
            }
            double? kilometers = distance / 1000;
            int averageFlightSpeedInKilometersPerHour = 800;
            double? durationInMinutes = (kilometers / averageFlightSpeedInKilometersPerHour) * 60;
            return ((int)kilometers, (int)durationInMinutes);
        }

        private decimal CalculateFlightPrice(int kilometers)
        {
            decimal pricePerKilometer = 0.07m;
            return pricePerKilometer * kilometers;

        }

        public async Task<FlightClass?> GetFlightClassById(int id)
        {
            var flightClass = await _flightRepository.GetFlightClassById(id);
            return flightClass;
        }

        public async Task CancelFlight(int flightId)
        {
            var deletedFlight = await _flightRepository.Delete(flightId);
            if (deletedFlight == null)
            {
                throw new Exception("Flight could not be found."); // Could potentially define more specific exceptions (EntityNotfoundException)
            }
            var passengers = deletedFlight.Tickets.Select(ticket => ticket.Passenger).ToList();
            try
            {
                await _emailService.SendFlightEmailAsync(passengers, FlightStatus.Cancelled);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception("An error occured while trying to send email to passengers regarding cancellation of flight.");
            }
        }
        public async Task ChangeFlight()
        {
            var dummyPassenger = new List<Passenger> { new() { Email = "" } };
            await _emailService.SendFlightEmailAsync(dummyPassenger, FlightStatus.Changed);
        }

    }
}
