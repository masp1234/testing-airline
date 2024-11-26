using backend.Dtos;
using backend.Models;

namespace backend.Repositories
{
    public interface IBookingRepository
    {
        Task<Booking> CreateBooking(BookingProcessedRequest bookingProcessedRequest);
    }
}
