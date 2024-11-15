using backend.Dtos;

namespace backend.Services
{
    public interface IAirlineService
    {

        Task<List<AirlineResponse>> GetAllAirlines();
    }
}