using AutoMapper;
using backend.Dtos;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IMapper _mapper;
        public FlightService(IFlightRepository flightRepository, IMapper mapper) {
            _flightRepository = flightRepository;
            _mapper = mapper;
            
                }

        public async Task<List<FlightResponse>> GetAllFlights()
        {
            var flights = await _flightRepository.GetAll();
            var mappedFlights = _mapper.Map<List<FlightResponse>>(flights);
            return mappedFlights;
        }
        public async Task<Flight> CreateFlight(FlightCreationRequest flightCreationRequest)
        {
            Flight flight = _mapper.Map<Flight>(flightCreationRequest);
            flight.FlightCode = "123FLIGHTCODE";
            flight.Kilometers = 123;
            flight.TravelTime = 240;
            Flight createdFlight = await _flightRepository.Create(flight);
            return createdFlight;

        }
    }
}
