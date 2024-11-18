using backend.Models;

namespace backend.Repositories
{
    public interface IAirplaneRepository
    {
        Task<List<Airplane>> GetAll();

        Task<Airplane?> GetAirplaneById(int id);
    }
}
