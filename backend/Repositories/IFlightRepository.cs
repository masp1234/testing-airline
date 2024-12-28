using backend.Dtos;
using backend.Models;

namespace backend.Repositories

{
    public interface IFlightRepository
    {
        Task<List<Flight>> GetAll();
        Task<Flight?> GetFlightById(long id);
        Task<Flight?> GetFlightWithRelationshipsById(long id);
        Task<Flight?> GetFlightByIdempotencyKey(string idempotencyKey);

        Task<List<Flight>> GetFlightsByAirplaneIdAndTimeInterval(Flight newFlight);
        Task<Flight> Create(Flight flight);

        Task<Flight> Delete(long id, string deletedBy);
        Task<List<Flight>> GetFlightsByDepartureDestinationAndDepartureDate(long departureAirportId, long destinationAirportId, DateOnly departureDate);

        Task<List<Flight>> GetFlightsByAirplaneId(long airplaneId);

        Task<FlightClass?> GetFlightClassById(long id);

        Task<List<Ticket>> GetTicketsByFlightId(long id);
        Task<bool> UpdateFlight(Flight flight);
    }
}
