using backend.Dtos;

namespace backend.Services
{
    public interface IAirplaneService
    {
        Task<List<AirplaneResponse>> GetAllAirplanes();
    }
}
