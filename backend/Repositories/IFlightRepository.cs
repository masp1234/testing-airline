using backend.Models;

namespace backend.Repositories

{
    public interface IFlightRepository
    {
        Task<List<Flight>> GetAll();
        Task<Flight> Create(Flight flight);
        Task<List<Flight>> GetFlightsByDepartureDestinationAndDepartureDate(int departureAirportId, int destinationAirportId, DateOnly departureDate);
    }
}
