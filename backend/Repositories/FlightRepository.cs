﻿using backend.Database;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace backend.Repositories
{
    public class FlightRepository: IFlightRepository
    {
        private readonly DatabaseContext _context;
        public FlightRepository(DatabaseContext context) {
            _context = context;
        }

        public async Task<List<Flight>> GetAll()
        {
            var flights = await _context.Flights
                // AsNoTracking gives better performance but should only be used in "read-only" scenarios
                .AsNoTracking()
                .Include(flight => flight.FlightsAirline)
                .Include(flight => flight.FlightsAirplane)
                .Include(flight => flight.DeparturePortNavigation)
                .Include(flight => flight.ArrivalPortNavigation)
                .ToListAsync();
            return flights;
        }

        public async Task<Flight?> GetFlightById(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            return flight;
        }

        public async Task<Flight?> GetFlightWithRelationshipsById(int id)
        {
            var flight = await _context.Flights
                .Include(flight => flight.FlightsAirline)
                .Include(flight => flight.FlightsAirplane)
                .Include(flight => flight.DeparturePortNavigation)
                .Include(flight => flight.ArrivalPortNavigation)
                .FirstOrDefaultAsync(flight => flight.Id == id);
            return flight;
        }

        public async Task<List<Flight>> GetFlightsByAirplaneIdAndTimeInterval(Flight newFlight)
        {
            var flights = await _context.Flights
                .Where(flight => flight.FlightsAirplaneId == newFlight.FlightsAirplaneId
                        && flight.DepartureTime < newFlight.CompletionTime
                        && flight.CompletionTime > newFlight.DepartureTime)
                .ToListAsync();
            return flights;
        }

        public async Task<Flight?> GetFlightByIdempotencyKey(string idempotencyKey)
        {
            Flight? flight = await _context.Flights.FirstOrDefaultAsync((flight) => flight.IdempotencyKey == idempotencyKey);
            return flight;
        }

        public async Task<Flight> Create(Flight flight)
        {
            var newFlight = new Flight
            {
                FlightCode = flight.FlightCode,
                DeparturePort = flight.DeparturePort,
                ArrivalPort = flight.ArrivalPort,
                DepartureTime = flight.DepartureTime,
                TravelTime = flight.TravelTime,
                Price = flight.Price,
                Kilometers = flight.Kilometers,
                EconomyClassSeatsAvailable = flight.EconomyClassSeatsAvailable,
                BusinessClassSeatsAvailable = flight.BusinessClassSeatsAvailable,
                FirstClassSeatsAvailable = flight.FirstClassSeatsAvailable,
                FlightsAirlineId = flight.FlightsAirlineId,
                FlightsAirplaneId = flight.FlightsAirplaneId,
                CompletionTime = flight.CompletionTime,
                IdempotencyKey = flight.IdempotencyKey
            };

            _context.Flights.Add(newFlight);
            await _context.SaveChangesAsync();



            return newFlight;

        }

        public async Task<Flight> Delete(int id)
        {

            var flight = await _context.Flights
                .Include(f => f.Tickets)
                    .ThenInclude(t => t.Passenger)
                .Include(f => f.Tickets)
                    .ThenInclude(t => t.TicketsBooking)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (flight == null)
            {
                return null;
            }

            if (flight.Tickets != null && flight.Tickets.Count != 0)
            {
                foreach (var ticket in flight.Tickets)
                {
                    // Remove the ticket
                    _context.Tickets.Remove(ticket);

                    // Removing related passenger
                    if (ticket.Passenger != null)
                    {
                         _context.Passengers.Remove(ticket.Passenger);
                    }

                    // Removing bookings if no other tickets reference it
                    if (ticket.TicketsBooking != null)
                    {
                        var hasOtherTicketsForBooking = await _context.Tickets.AnyAsync(t => t.TicketsBookingId == ticket.TicketsBooking.Id && t.Id != ticket.Id);
                        if (!hasOtherTicketsForBooking)
                        {
                            _context.Bookings.Remove(ticket.TicketsBooking);
                        }
                    }
                }
            }

            // Removing flight
            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();

            return flight;
        }


        public async Task<List<Flight>> GetFlightsByDepartureDestinationAndDepartureDate(int departureAirportId, int destinationAirportId, DateOnly departureDate)
        {
            var flights = await _context.Flights
                .Where(flight =>
                       flight.DeparturePort == departureAirportId &&
                       flight.ArrivalPort == destinationAirportId &&
                       DateOnly.FromDateTime(flight.DepartureTime) == departureDate
                    )
                .Include(flight => flight.FlightsAirline)
                .Include(flight => flight.DeparturePortNavigation)
                .Include(flight => flight.ArrivalPortNavigation)
                .ToListAsync();
            return flights;
        }

        public async Task<List<Flight>> GetFlightsByAirplaneId(int airplaneId)
        {
            var flights = await _context.Flights
                .Where(flight => flight.FlightsAirplaneId == airplaneId)
                .ToListAsync();

            return flights;
        }

        public async Task<FlightClass?> GetFlightClassById(int id)
        {
            var flightClass = await _context.FlightClasses.FindAsync(id);
            return flightClass;
        }

        public async Task<List<Ticket>> GetTicketsByFlightId(int flightId)
        {
            var tickets = await _context.Tickets.Where(ticket => ticket.FlightId == flightId)
                .Include(ticket => ticket.Passenger)
                .ToListAsync();
            return tickets;
        }

        public async Task<bool> UpdateFlight(Flight flightToUpdate)
        {
            try
            {
                _context.Flights.Update(flightToUpdate);
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
