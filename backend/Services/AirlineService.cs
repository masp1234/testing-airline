using AutoMapper;
using backend.Dtos;
using backend.Repositories;

namespace backend.Services
{
    public class AirlineService(IAirlineRepository airlineRepository,
                                IMapper mapper): IAirlineService
    {
        private readonly IAirlineRepository _airlineRepository = airlineRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<List<AirlineResponse>> GetAllAirlines()
        {
            var airlines = await _airlineRepository.GetAll();
            var mappedAirlines = _mapper.Map<List<AirlineResponse>>(airlines);
            return mappedAirlines;
        }
    }
}