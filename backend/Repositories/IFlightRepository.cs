using backend.Models;

namespace backend.Repositories

{
    public interface IFlightRepository
    {
        public Task<List<Flight>> GetAll();
        public Task<Flight> Create(Flight flight);


    }
}
