using AutoMapper;
using backend.Database;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories.MongoDB
{
    public class AirportMongoDBRepository(MongoDBContext context, IMapper mapper): IAirportRepository
    {
        private readonly MongoDBContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<List<Airport>> FindByIds(params long[] ids)
        {
            var airports = await _context.Airports
                .Where(airport => ids.Contains(airport.Id))
                .ToListAsync();
            return _mapper.Map<List<Airport>>(airports);
        }

        public async Task<List<Airport>> GetAll()
        {
            var airports = await _context.Airports.ToListAsync();
            return _mapper.Map<List<Airport>>(airports);
        }
    }
}
