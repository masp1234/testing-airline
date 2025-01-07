using backend.Dtos;
using backend.Models;

namespace backend.Repositories
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetBookingsByUserId(long id);
        Task<Booking> CreateBooking(BookingProcessedRequest bookingProcessedRequest);
    }
}
