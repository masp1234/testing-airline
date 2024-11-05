using backend.Database;

namespace backend.Repositories
{
    public class FlightRepository: IFlightRepository
    {
        private readonly DatabaseContext _context;
        public FlightRepository(DatabaseContext context) {
            _context = context;
        }

        public async Task Create(Flight flight)
        {
            await _context.Flights.AddAsync(flight);
            await _context.SaveChangesAsync();
        }
    }
}
