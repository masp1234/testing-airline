using backend.Database;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class AirplaneRepository(DatabaseContext context): IAirplaneRepository
    {
        private readonly DatabaseContext _context = context;
        public async Task<List<Airplane>> GetAll()
        {
            var airplanes = await _context.Airplanes.ToListAsync();
            return airplanes;
        }

        public async Task<Airplane?> GetAirplaneById(int id)
        {
            var airplane = await _context.Airplanes.FindAsync(id);
            return airplane;
        }
    }
}
