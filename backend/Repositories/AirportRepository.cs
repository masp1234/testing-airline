using backend.Database;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class AirportRepository(DatabaseContext context): IAirportRepository
    {
        private readonly DatabaseContext _context = context;
        public async Task<List<Airport>> GetAll()
        {
            var airports = await _context.Airports
                    .Include(airport => airport.City)
                    .ThenInclude(city => city.State)
                    .ToListAsync();
            return airports;
        }

        public async Task<List<Airport>> FindByIds(params long[] ids)
        {
            var airports = await _context.Airports
                .Where((airport) => ids.Contains(airport.Id))
                .ToListAsync();

            return airports;
        }
    }
}
