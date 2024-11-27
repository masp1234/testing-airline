using AutoMapper;
using backend.Dtos;
using backend.Models;
using backend.Repositories;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace backend.Services
{
    public class BookingService(IUserService userService, IFlightService flightService, IBookingRepository bookingRepository, IMapper mapper) : IBookingService
    {
        private readonly IUserService _userService = userService;
        private readonly IFlightService _flightService = flightService;
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<ServiceResult<Booking>> CreateBooking(BookingCreationRequest bookingCreationRequest)
        {
            Dictionary<(int, string), int> ticketClassesPerFlight = [];
            HashSet<Flight> uniqueFlights = [];
            var user = await _userService.GetUserByEmail(bookingCreationRequest.Email);
            if (user == null)
            {
                return ServiceResult<Booking>.Failure($"No user found with the email: {bookingCreationRequest.Email}.");
            }

            // Map the CreationRequest DTO to ProcessedRequest DTO to allow for more properties without polluting the CreationRequest DTO
            BookingProcessedRequest bookingProcessedRequest = _mapper.Map<BookingProcessedRequest>(bookingCreationRequest);

            foreach (var ticket in bookingProcessedRequest.Tickets)
            {
                var flight = await _flightService.GetFlightById(ticket.FlightId);

                if (flight == null)
                {
                    return ServiceResult<Booking>.Failure($"No flight found with the id: {ticket.FlightId}.");
                }

                var flightClass = await _flightService.GetFlightClassById(ticket.FlightClassId);

                if (flightClass == null)
                {
                    return ServiceResult<Booking>.Failure($"No flight class found with the id: {ticket.FlightClassId}.");
                }

                
                ticket.FlightClassName = flightClass.Name;
                ticket.TicketNumber = GenerateUniqueString();
                ticket.FlightPrice = CalculateTicketPrice(flight, flightClass);
                var flightIdAndClassName = (ticket.FlightId, flightClass.Name);
                if (ticketClassesPerFlight.ContainsKey(flightIdAndClassName))
                {
                    ticketClassesPerFlight[flightIdAndClassName] += 1;
                }
                else
                {
                    ticketClassesPerFlight[flightIdAndClassName] = 1;
                }
                uniqueFlights.Add(flight);

                
            }

            bool ticketIsAvailable = CheckTicketAvailability(uniqueFlights, ticketClassesPerFlight);
            if (!ticketIsAvailable)
            {
                return ServiceResult<Booking>.Failure($"Some of the tickets requested are unavailable.");
            }

            bookingProcessedRequest.ConfirmationNumber = GenerateUniqueString();
            bookingProcessedRequest.UserId = user.Id;
            var booking = await _bookingRepository.CreateBooking(bookingProcessedRequest);
            return ServiceResult<Booking>.Success(booking, "The booking was created successfully");
        }
        private decimal CalculateTicketPrice(Flight flight, FlightClass flightClass)
        {
            decimal multiplier = flightClass.Name switch
            {
                "Economy" => 1.00m,
                "Business" => 1.50m,
                "First Class" => 3.00m,
                _ => 0
            };

            decimal ticketPrice = multiplier * flight.Price;
            return ticketPrice;
        }

        private string GenerateUniqueString()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");

            int randomStringLength = 6;
            Random random = new Random();
            string stringPart = "";

            char[] characters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                              'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                              '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            for (int i = 0; i < randomStringLength; i++)
            {
                int randomNumber = random.Next(0, characters.Length);
                stringPart += characters[randomNumber];
            }
            string bookingConfirmationNumber = datePart + "-" + stringPart;
            return bookingConfirmationNumber;
        }

        private bool CheckTicketAvailability(HashSet<Flight> flights, Dictionary<(int flightId, string flightClass), int> ticketClassesPerFlight)
        {
            foreach (Flight flight in flights)
            {
                foreach (var key in ticketClassesPerFlight.Keys)
                {
                    if (key.flightId == flight.Id)
                    {
                        string flightClass = key.flightClass;
                        int amountOfTickets = ticketClassesPerFlight[key];

                        bool ticketIsAvailable = flightClass switch
                        {
                            "Economy" => flight.EconomyClassSeatsAvailable - amountOfTickets >= 0,
                            "Business" => flight.BusinessClassSeatsAvailable - amountOfTickets >= 0,
                            "First Class" => flight.FirstClassSeatsAvailable - amountOfTickets >= 0,
                            _ => throw new ArgumentException($"'{flightClass}' is not a valid flight class")
                        };

                        if (!ticketIsAvailable)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

    }
}