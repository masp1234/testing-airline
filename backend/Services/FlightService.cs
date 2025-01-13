﻿using AutoMapper;
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
        IAirplaneService airplaneService
            ) : IFlightService
    {
        private readonly IFlightRepository _flightRepository = flightRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IDistanceApiService _distanceApiService = distanceApiService;
        private readonly IAirportRepository _airportRepository = airportRepository;
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
                throw new InvalidDataException("The chosen airplane could not be found.");
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
            flight.TravelTime = duration;
            flight.CompletionTime = CalculateFlightCompletionTime(flight.DepartureTime, flight.TravelTime);

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

        private DateTime CalculateFlightCompletionTime(DateTime departureTime, int flightDurationInMinutes)
        {
            // the ´variable below is meant to simulate preparation time in minutes between when an airplane lands at an airport, and when it is ready to fly again.
            int simulatedPreparationTimeInMinutes = 120;
            return departureTime.AddMinutes(flightDurationInMinutes + simulatedPreparationTimeInMinutes);
        }

        public async Task<FlightClass?> GetFlightClassById(int id)
        {
            var flightClass = await _flightRepository.GetFlightClassById(id);
            return flightClass;
        }

        public async Task<bool> UpdateFlight(UpdateFlightRequest updateFlightRequest, Flight flight)
        {
            
            flight.DepartureTime = updateFlightRequest.DepartureDateTime;
            flight.CompletionTime = CalculateFlightCompletionTime(flight.DepartureTime, flight.TravelTime);

            var overLappingFlights = await _flightRepository.GetFlightsByAirplaneIdAndTimeInterval(flight);
            
            if (overLappingFlights.Count > 0 && overLappingFlights.Any(f => f.Id != flight.Id))
            {
                throw new Exception("There was 1 or more overlapping flights.");
            }

            bool updatedSuccessfully = await _flightRepository.UpdateFlight(flight);
            if (updatedSuccessfully)
            {
                var flightTickets = await _flightRepository.GetTicketsByFlightId(flight.Id);
                var flightPassengers = flightTickets
                    .Select(ticket => ticket.Passenger)
                    .ToList();
                return true;
            }
            return false;
        }
        
        public async Task CancelFlight(int flightId)
        {
            var deletedFlight = await _flightRepository.Delete(flightId);
            if (deletedFlight == null)
            {
                throw new Exception("Flight could not be found."); // Could potentially define more specific exceptions (EntityNotfoundException)
            }
            
        }
    }
}
