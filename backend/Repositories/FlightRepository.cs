using backend.Database;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class FlightRepository: IFlightRepository
    {
        private readonly DatabaseContext _context;
        public FlightRepository(DatabaseContext context) {
            _context = context;
        }

        public async Task<Flight> Create(Flight newFlight)
        {
            // Check if a flight with the same idempotency key has already been added. If it has, return it instead of creating a new one
            Flight? existingFlight = await _context.Flights.FirstOrDefaultAsync((flight) => flight.IdempotencyKey == newFlight.IdempotencyKey);
            if (existingFlight != null)
            {
                return existingFlight;
            }
            await _context.Flights.AddAsync(newFlight);
            await _context.SaveChangesAsync();
            return newFlight;
        }
    }
}
