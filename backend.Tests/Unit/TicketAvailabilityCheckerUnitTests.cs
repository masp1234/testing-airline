
using backend.Models;
using backend.Services;

namespace backend.Tests.Unit
{
    public class TicketAvailabilityCheckerUnitTests
    {
        [Theory]
        [InlineData(0, 1, 1, 0, FlightClassName.EconomyClass, true)]  // Lower boundary: 0 tickets requested
        [InlineData(1, 1, 1, 1, FlightClassName.EconomyClass, true)]  // Exact boundary: 1 seat available, 1 ticket requested
        [InlineData(1, 1, 1, 2, FlightClassName.EconomyClass, false)] // Upper boundary: Request exceeds availability
        [InlineData(1, 0, 1, 0, FlightClassName.BusinessClass, true)] // Lower boundary: 0 tickets requested
        [InlineData(1, 1, 1, 1, FlightClassName.BusinessClass, true)] // Exact boundary: 1 seat available, 1 ticket requested
        [InlineData(1, 1, 1, 2, FlightClassName.BusinessClass, false)] // Upper boundary: Request exceeds availability
        [InlineData( 1, 1, 0, 0, FlightClassName.FirstClass, true)]    // Lower boundary: 0 tickets requested
        [InlineData( 1, 1, 1, 1, FlightClassName.FirstClass, true)]    // Exact boundary: 1 seat available, 1 ticket requested
        [InlineData( 1, 1, 1, 2, FlightClassName.FirstClass, false)]   // Upper boundary: Request exceeds availability
        public void CheckTicketAvailability_WithDifferentAmountOfTicketsAvailable_AndDifferentAmount_Of_RequestedTickets(
            int economySeats,
            int businessSeats,
            int firstClassSeats,
            int requestedTickets,
            FlightClassName flightClassName,
            bool expectedResult)
        {
            // Arrange
            var flight = new Flight()
            {
                Id = 1,
                EconomyClassSeatsAvailable = economySeats,
                BusinessClassSeatsAvailable = businessSeats,
                FirstClassSeatsAvailable = firstClassSeats,
            };

            var ticketAvailabilityChecker = new TicketAvailabilityChecker();
            ticketAvailabilityChecker.AddFlight(flight);

            for (int i = 0; i < requestedTickets; i++)
            {
                ticketAvailabilityChecker.AddTicketForFlightIdAndFlightClass(flight.Id, flightClassName);
            }

            // Act
            bool ticketsAreAvailable = ticketAvailabilityChecker.CheckTicketAvailability();

            // Assert
            Assert.Equal(expectedResult, ticketsAreAvailable);
        }

        [Fact]
        public void CheckTicketAvailability_ShouldThrowException_WhenCalledWithInvalidFlightClass()
        {
            var flight = new Flight()
            {
                Id = 1,
                EconomyClassSeatsAvailable = 2,
                BusinessClassSeatsAvailable = 2,
                FirstClassSeatsAvailable = 2,
            };

            var ticketAvailabilityChecker = new TicketAvailabilityChecker();
            ticketAvailabilityChecker.AddFlight(flight);

            // Cast an invalid value to the FlightClassName enum
            var invalidFlightClass = (FlightClassName)999;

            ticketAvailabilityChecker.AddTicketForFlightIdAndFlightClass(flight.Id, invalidFlightClass);

            var exception = Assert.Throws<ArgumentException>(() => ticketAvailabilityChecker.CheckTicketAvailability());
            Assert.Contains("not a valid flight class", exception.Message);
        }

    }
}
