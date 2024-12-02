using backend.Models;
using backend.Enums;

namespace backend.Services
{
    public interface IEmailService
    {
        Task SendFlightEmailAsync(List<Passenger> passengers, FlightStatus status);
    }
}