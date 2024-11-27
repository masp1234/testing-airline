using backend.Dtos;
using backend.Models;

namespace backend.Services
{
    public interface IBookingService
    {
        Task<ServiceResult<Booking>> CreateBooking(BookingCreationRequest bookingCreationRequest);
    }
}
