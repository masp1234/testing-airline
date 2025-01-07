using backend.Models;

namespace backend.Services
{
    public interface ITicketAvailabilityChecker
    {
        public void AddFlight(Flight flight);
        public void AddAmountOfTicketsForFlightIdAndFlightClass(long flightId, FlightClassName flightClassName);
        bool CheckTicketAvailability();
    }
}
