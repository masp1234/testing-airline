using AutoMapper;
using backend.Dtos;
using backend.Repositories;

namespace backend.Services
{
    public class AirplaneService(IAirplaneRepository airplaneRepository, IMapper mapper) : IAirplaneService
    {
        private readonly IAirplaneRepository _airplaneRepository = airplaneRepository;
        private readonly IMapper _mapper = mapper;
        public async Task<List<AirplaneResponse>> GetAllAirplanes()
        {
            var airplanes = await _airplaneRepository.GetAll();
            var mappedAirplanes = _mapper.Map<List<AirplaneResponse>>(airplanes);
            return mappedAirplanes;
        }
    }
}
