using backend.Database;
using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;
namespace backend.Repositories
{
    public class BookingRepository(DatabaseContext context) : IBookingRepository
    {
        public async Task<List<Booking>> GetBookingsByUserId(int id)
        {
            var bookings = await _context.Bookings
                .Where(booking => booking.UserId == id)
                .ToListAsync();
            return bookings;
        }
        private readonly DatabaseContext _context = context;
        public async Task<Booking> CreateBooking(BookingProcessedRequest request)
        {
            var transaction = _context.Database.BeginTransaction();

            try
            {
                var createdBooking = await _context.Bookings.AddAsync(new Booking()
                {
                    ConfirmationNumber = request.ConfirmationNumber,
                    UserId = request.UserId,

                });

                await _context.SaveChangesAsync();

                foreach (var ticket in request.Tickets)
                {
                    var passenger = await _context.Passengers.AddAsync(new Passenger()
                    {
                        FirstName = ticket.Passenger.FirstName,
                        LastName = ticket.Passenger.LastName,
                        Email = ticket.Passenger.Email,
                    });

                    var createdTicket = await _context.Tickets.AddAsync(new Ticket()
                    {
                        FlightClassId = ticket.FlightClassId,
                        FlightId = ticket.FlightId,
                        Passenger = passenger.Entity,
                        Price = ticket.FlightPrice,
                        TicketNumber = ticket.TicketNumber,
                        TicketsBookingId = createdBooking.Entity.Id
                    });

                    var flight = await context.Flights.FindAsync(ticket.FlightId);
                    flight?.DecrementSeatAvailability(ticket.FlightClassName);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return createdBooking.Entity;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;

            }
        }
        
    }
}
