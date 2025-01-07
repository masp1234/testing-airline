using AutoMapper;
using backend.Database;
using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories.MongoDB
{
    public class BookingMongoDBRepository(MongoDBContext context, IMapper mapper): IBookingRepository
    {
        private readonly MongoDBContext _context = context;
        private readonly IMapper _mapper = mapper;

        public Task<Booking> CreateBooking(BookingProcessedRequest bookingProcessedRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Booking>> GetBookingsByUserId(long id)
        {
            var bookings = await _context.Bookings.Where(booking => booking.User.Id == id)
                .ToListAsync();
            return _mapper.Map<List<Booking>>(bookings);
        }
    }


}
