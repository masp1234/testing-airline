using backend.Models;

namespace backend.Repositories

{
    public interface IFlightRepository
    {
        Task<List<Flight>> GetAll();
        Task<Flight?> GetFlightById(int id);
        Task<Flight?> GetFlightWithRelationshipsById(int id);
        Task<Flight?> GetFlightByIdempotencyKey(string idempotencyKey);

        Task<List<Flight>> GetFlightsByAirplaneIdAndTimeInterval(Flight newFlight);
        Task<Flight> Create(Flight flight);

        Task<Flight> Delete(int id);
        Task<List<Flight>> GetFlightsByDepartureDestinationAndDepartureDate(int departureAirportId, int destinationAirportId, DateOnly departureDate);

        Task<List<Flight>> GetFlightsByAirplaneId(int airplaneId);

        Task<FlightClass?> GetFlightClassById(int id);
    }
}
