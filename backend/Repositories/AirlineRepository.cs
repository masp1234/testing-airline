using backend.Database;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class AirlineRepository(DatabaseContext context): IAirlineRepository
    {
        private readonly DatabaseContext _context = context;
        public async Task<List<Airline>> GetAll()
        {
            var airlines = await _context.Airlines.ToListAsync();
            return airlines;
        }
    }
}