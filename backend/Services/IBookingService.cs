using backend.Dtos;
using backend.Models;

namespace backend.Services
{
    public interface IBookingService
    {
        Task<ServiceResult<List<BookingResponse>>> GetBookingsByUserEmail(string email);
        Task<ServiceResult<BookingResponse>> CreateBooking(BookingCreationRequest bookingCreationRequest);
    }
}
