using backend.Dtos;

namespace backend.Services
{
    public interface IFlightService
    {

        public Task CreateFlight(FlightCreationRequest flightCreationRequest);
    }
}
