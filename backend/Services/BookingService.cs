using backend.Dtos;
using backend.Models;

namespace backend.Services
{
    public class BookingService(IUserService userService, IFlightService flightService) : IBookingService
    {
        private readonly IUserService _userService = userService;
        private readonly IFlightService _flightService = flightService;

        public async Task<ServiceResult<Booking>> CreateBooking(BookingCreationRequest bookingCreationRequest)
        {
            var user = await _userService.GetUserByEmail(bookingCreationRequest.Email);
            if (user == null)
            {
                return ServiceResult<Booking>.Failure($"No user found with the email: {bookingCreationRequest.Email}.");
            }
            foreach (var ticket in bookingCreationRequest.Tickets)
            {
                var flight = await _flightService.GetFlightById(ticket.FlightId);

                if (flight == null)
                {
                    return ServiceResult<Booking>.Failure($"No flight found with the id: {ticket.FlightId}.");
                }

                var flightClass = await _flightService.GetFlightClassById(ticket.FlightClassId);

                if (flightClass == null)
                {
                    return ServiceResult<Booking>.Failure($"No flight class found with the id: {ticket.FlightClassId}.");
                }
            }
            return ServiceResult<Booking>.Failure($"ddsksd.");
        }
    }
}
