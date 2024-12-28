using backend.Database;
using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using backend.Config;
using AutoMapper;
namespace backend.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IMapper _mapper;
        private readonly DatabaseContext _context;
        public BookingRepository(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<Booking>> GetBookingsByUserId(long id)
        {
            var bookings = await _context.Bookings
                .Where(booking => booking.UserId == id)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Passenger)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Flight)
                        .ThenInclude(f => f.DeparturePortNavigation)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Flight)                            
                        .ThenInclude(f => f.ArrivalPortNavigation)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.FlightClass)
                .ToListAsync();

            return bookings;
        }

        public async Task<Booking> CreateBooking(BookingProcessedRequest request)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

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

                    var flight = await _context.Flights.FindAsync(ticket.FlightId);
                    flight?.DecrementSeatAvailability(ticket.FlightClassName);
                    flight.Version++;
       
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
