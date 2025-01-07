using AutoMapper;
using backend.Database;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace backend.Repositories.MongoDB
{
    public class FlightsMongoDBRepository(MongoDBContext context, IMapper mapper) : IFlightRepository
    {
        private readonly MongoDBContext _context = context;
        private readonly IMapper _mapper = mapper;
        public async Task<Flight> Create(Flight flight)
        {
            var overLappingFlights = await GetFlightsByAirplaneIdAndTimeInterval(flight);
            if (overLappingFlights.Any())
            {
                throw new InvalidOperationException("There are 1 or more overlapping flights.");
            }

            return null;
        }

        public Task<Flight> Delete(long id, string deletedBy)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Flight>> GetAll()
        {
            var flights = await _context.Flights.ToListAsync();
            return _mapper.Map<List<Flight>>(flights);
        }

        public async Task<Flight?> GetFlightById(long id)
        {
            var flight = await _context.Flights.FindAsync(id);
            return _mapper.Map<Flight>(flight);
        }

        public async Task<Flight?> GetFlightByIdempotencyKey(string idempotencyKey)
        {
            var flight = await _context.Flights.FirstOrDefaultAsync(flight => flight.IdempotencyKey == idempotencyKey);
            return _mapper.Map<Flight>(flight);
        }

        public async Task<FlightClass?> GetFlightClassById(long id)
        {
            var flight = await _context.FlightClasses.FindAsync(id);
            return _mapper.Map<FlightClass>(flight);
        }

        public async Task<List<Flight>> GetFlightsByAirplaneId(long airplaneId)
        {
            var flights = await _context.Flights.Where(flight => flight.FlightsAirplane.Id == airplaneId).ToListAsync();
            return _mapper.Map<List<Flight>>(flights);
        }

        public async Task<List<Flight>> GetFlightsByAirplaneIdAndTimeInterval(Flight newFlight)
        {
            var flights = await _context.Flights
                .Where(flight => flight.FlightsAirplane.Id == newFlight.FlightsAirplaneId
                        && flight.DepartureTime < newFlight.CompletionTime
                        && flight.CompletionTime > newFlight.DepartureTime)
                .ToListAsync();
            return _mapper.Map<List<Flight>>(flights);
        }

        public async Task<List<Flight>> GetFlightsByDepartureDestinationAndDepartureDate(long departureAirportId, long destinationAirportId, DateOnly departureDate)
        {
            var flights = await _context.Flights
                .Where(flight =>
                       flight.DeparturePort.Id == departureAirportId &&
                       flight.ArrivalPort.Id == destinationAirportId &&
                       DateOnly.FromDateTime(flight.DepartureTime) == departureDate
                    )
                .ToListAsync();
            return _mapper.Map<List<Flight>>(flights);
        }

        public async Task<Flight?> GetFlightWithRelationshipsById(long id)
        {
            var flight = await _context.Flights.FindAsync(id);
            return _mapper.Map<Flight>(flight);
        }

        public async Task<List<Ticket>> GetTicketsByFlightId(long flightId)
        {
            var tickets = await _context.Bookings
                .Where(b => b.Tickets.Any(t => t.Flight.Id == flightId))
                .SelectMany(b => b.Tickets.Where(t => t.Flight.Id == flightId))
                .ToListAsync();

            return _mapper.Map<List<Ticket>>(tickets);
        }


        public Task<bool> UpdateFlight(Flight flight)
        {
            throw new NotImplementedException();
        }
    }
}
