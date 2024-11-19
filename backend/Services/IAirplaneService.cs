using backend.Dtos;
using backend.Models;

namespace backend.Services
{
    public interface IAirplaneService
    {
        Task<List<AirplaneResponse>> GetAllAirplanes();

        Task<Airplane?> GetAirplaneById(int id);

        Task<List<AirplaneBookedTimeSlot>> GetBookedTimeslotsByAirplaneId(int id);
    }
}
