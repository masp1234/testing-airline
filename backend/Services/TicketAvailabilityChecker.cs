using backend.Models;

namespace backend.Services
{

    public class TicketAvailabilityChecker: ITicketAvailabilityChecker
    {
        private readonly Dictionary<(int flightId, string flightClass), int> ticketClassesPerFlight = [];
        HashSet<Flight> uniqueFlights = [];

        // Adds the flights that need to be checked - uses a HashSet to only add unique flights
        public void AddFlight(Flight flight)
        {
            uniqueFlights.Add(flight);
        }

        // Adds together the ticket amount per flight class and id
        public void AddAmountOfTicketsForFlightIdAndFlightClass(int flightId, string flightClassName)
        {
            var flightIdAndClassName = (flightId, flightClassName);
            if (ticketClassesPerFlight.ContainsKey(flightIdAndClassName))
            {
                ticketClassesPerFlight[flightIdAndClassName] += 1;
            }
            else
            {
                ticketClassesPerFlight[flightIdAndClassName] = 1;
            }
        }

        // Loops through the unique flights and checks that each flight class have enough tickets available for all of the tickets
        // As soon as one flight class does not have enough tickets, it returns false.
        public bool CheckTicketAvailability()
        {
            foreach (Flight flight in uniqueFlights)
            {
                foreach (var key in ticketClassesPerFlight.Keys)
                {
                    if (key.flightId == flight.Id)
                    {
                        string flightClass = key.flightClass;
                        int amountOfTickets = ticketClassesPerFlight[key];

                        bool ticketIsAvailable = flightClass switch
                        {
                            "Economy" => flight.EconomyClassSeatsAvailable - amountOfTickets >= 0,
                            "Business" => flight.BusinessClassSeatsAvailable - amountOfTickets >= 0,
                            "First Class" => flight.FirstClassSeatsAvailable - amountOfTickets >= 0,
                            _ => throw new ArgumentException($"'{flightClass}' is not a valid flight class")
                        };

                        if (!ticketIsAvailable)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
