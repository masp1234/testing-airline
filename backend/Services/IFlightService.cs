using backend.Dtos;
using backend.Models;

namespace backend.Services
{
    public interface IFlightService
    {
        Task<List<FlightResponse>> GetAllFlights();

        Task<Flight?> GetFlightById(long id);

        Task<FlightResponse?> GetFlightWithRelationshipsById(long id);
        Task<Flight> CreateFlight(FlightCreationRequest flightCreationRequest);
        Task<List<FlightResponse>> GetFlightsByDepartureDestinationAndDepartureDate(long departureAirportId, long destinationAirportId, DateOnly departureDate);
        Task<FlightClass?> GetFlightClassById(long id);
        Task<bool> UpdateFlight(UpdateFlightRequest updateFlightRequest, Flight flight);
        Task CancelFlight(long id, string canceledBy);
        Task ChangeFlight();
    }
}
