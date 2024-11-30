using backend.Dtos;
using backend.Models;

namespace backend.Services
{
    public interface IBookingService
    {
        Task<ServiceResult<BookingCreationResponse>> CreateBooking(BookingCreationRequest bookingCreationRequest);
    }
}
