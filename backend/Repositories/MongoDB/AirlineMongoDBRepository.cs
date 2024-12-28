using AutoMapper;
using backend.Database;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories.MongoDB
{
    public class AirlineMongoDBRepository(MongoDBContext context, IMapper mapper) : IAirlineRepository
    {
        private readonly MongoDBContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<List<Airline>> GetAll()
        {
            var airlines = await _context.Airlines.ToListAsync();
            return _mapper.Map<List<Airline>>(airlines);
        }
    }
}