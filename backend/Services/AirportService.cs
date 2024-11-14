using AutoMapper;
using backend.Dtos;
using backend.Repositories;

namespace backend.Services
{
    public class AirportService(IAirportRepository airportRepository,
                                IMapper mapper): IAirportService
    {
        private readonly IAirportRepository _airportRepository = airportRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<List<AirportResponse>> GetAllAirports()
        {
            var airports = await _airportRepository.GetAll();
            var mappedAirports = _mapper.Map<List<AirportResponse>>(airports);
            return mappedAirports;
        }
    }
}
