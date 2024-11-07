using backend.Models;

namespace backend.Repositories

{
    public interface IFlightRepository
    {
        public Task<Flight> Create(Flight flight);
    }
}
