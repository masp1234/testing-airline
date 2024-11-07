using backend.Dtos;
using backend.Models;

namespace backend.Services
{
    public interface IFlightService
    {

        public Task<Flight> CreateFlight(FlightCreationRequest flightCreationRequest);
    }
}
