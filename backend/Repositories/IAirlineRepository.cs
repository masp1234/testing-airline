using backend.Models;

namespace backend.Repositories
{
    public interface IAirlineRepository
    {
        Task<List<Airline>> GetAll();
    }
}