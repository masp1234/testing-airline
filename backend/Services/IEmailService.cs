using backend.Models;
using backend.Enums;
using backend.Dtos;

namespace backend.Services
{
    public interface IEmailService
    {
        Task SendFlightEmailAsync(List<Passenger> passengers, FlightStatus status);
        Task SendBookingConfirmationMail(BookingProcessedRequest booking);
    }
}