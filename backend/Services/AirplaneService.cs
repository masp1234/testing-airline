using AutoMapper;

using backend.Dtos;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class AirplaneService(IAirplaneRepository airplaneRepository, IFlightRepository flightRepository, IMapper mapper) : IAirplaneService
    {
        private readonly IAirplaneRepository _airplaneRepository = airplaneRepository;
        private readonly IFlightRepository _flightRepository = flightRepository;
        private readonly IMapper _mapper = mapper;
        public async Task<List<AirplaneResponse>> GetAllAirplanes()
        {
            var airplanes = await _airplaneRepository.GetAll();
            var mappedAirplanes = _mapper.Map<List<AirplaneResponse>>(airplanes);
            return mappedAirplanes;
        }

        public async Task<Airplane?> GetAirplaneById(int id)
        {
            return await _airplaneRepository.GetAirplaneById(id);
        }

        public async Task<List<AirplaneBookedTimeSlot>> GetBookedTimeslotsByAirplaneId(int id)
        {
            var flights = await _flightRepository.GetFlightsByAirplaneId(id);
            var bookedTimeSlots = GetBookedTimeSlots(flights);

            return bookedTimeSlots;
        }

        private List<AirplaneBookedTimeSlot> GetBookedTimeSlots(List<Flight> flights)
        {
            List<AirplaneBookedTimeSlot> bookedTimeSlots = flights.Select((flight) =>
            {
                return new AirplaneBookedTimeSlot()
                {
                    FlightId = flight.Id,
                    TimeSlotStart = flight.DepartureTime,
                    TimeSlotEnd = flight.CompletionTime
                };
            }).ToList();

            return bookedTimeSlots;
        }
    }
}
