using backend.Models;
using backend.Models.MongoDB;

namespace backend.Repositories
{
    public interface IAirplaneRepository
    {
        Task<List<Airplane>> GetAll();

        Task<Airplane?> GetAirplaneById(long id);
    }
}
