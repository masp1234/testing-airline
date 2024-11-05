using AutoMapper;
using backend.Dtos;
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
        public async Task CreateFlight(FlightCreationRequest flightCreationRequest)
        {
            Flight flight = _mapper.Map<Flight>(flightCreationRequest);
            Console.WriteLine(flight);
            await _flightRepository.Create(flight);

        }
    }
}
