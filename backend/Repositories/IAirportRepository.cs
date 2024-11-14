using backend.Models;

namespace backend.Repositories
{
    public interface IAirportRepository
    {
        Task<List<Airport>> GetAll();
    }
}
