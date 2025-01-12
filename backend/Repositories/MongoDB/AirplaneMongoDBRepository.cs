using AutoMapper;
using backend.Database;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories.MongoDB
{
    public class AirplaneMongoDBRepository(MongoDBContext context, IMapper mapper) : IAirplaneRepository
    {
        private readonly MongoDBContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<Airplane?> GetAirplaneById(long id)
        {
            var airplane = await _context.Airplanes.FindAsync(id);
            var mappedAirplane = _mapper.Map<Airplane>(airplane);
            return mappedAirplane;
        }

        public async Task<List<Airplane>> GetAll()
        {
            var airplanes = await _context.Airplanes.ToListAsync();
            return _mapper.Map<List<Airplane>>(airplanes);
        }
    }
}
