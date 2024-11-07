using backend.Dtos;

namespace backend.Services
{
    public interface IAirportService
    {

        Task<List<AirportResponse>> GetAllAirports();
    }
}