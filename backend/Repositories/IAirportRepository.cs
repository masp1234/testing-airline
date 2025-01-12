using backend.Models;

namespace backend.Repositories
{
    public interface IAirportRepository
    {
        Task<List<Airport>> GetAll();

        Task<List<Airport>> FindByIds(params long[] ids);
    }
}
