using AutoMapper;
using backend.Dtos;
using backend.Models;
using backend.Repositories;
using backend.Utils;
using MongoDB.Bson;

namespace backend.Services
{
    public class BookingService(
        IUserService userService,
        IFlightService flightService,
        IEmailService emailService,
        IBookingRepository bookingRepository,
        IMapper mapper,
        ITicketAvailabilityChecker ticketAvailabilityChecker
        ) : IBookingService
    {
        private readonly IUserService _userService = userService;
        private readonly IFlightService _flightService = flightService;
        private readonly IEmailService _emailService = emailService;
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ITicketAvailabilityChecker _ticketAvailabilityChecker = ticketAvailabilityChecker;

        public async Task<ServiceResult<List<BookingResponse>>> GetBookingsByUserEmail(string email)
        {
            var user = await _userService.GetUserByEmail(email);
            if (user == null) {
                return ServiceResult<List<BookingResponse>>.Failure($"No user found with the email {email}");
            }

            var bookings = await _bookingRepository.GetBookingsByUserId(user.Id);
            //Console.WriteLine(bookings.ToJson());
            var bookingResponse = _mapper.Map<List<BookingResponse>>(bookings);
            return ServiceResult<List<BookingResponse>>.Success(bookingResponse);
        }

        public async Task<ServiceResult<BookingResponse>> CreateBooking(BookingCreationRequest bookingCreationRequest)
        {
            var user = await _userService.GetUserByEmail(bookingCreationRequest.Email);
            if (user == null)
            {
                return ServiceResult<BookingResponse>.Failure($"No user found with the email: {bookingCreationRequest.Email}.");
            }

            // Map the CreationRequest DTO to ProcessedRequest DTO to allow for more properties without polluting the CreationRequest DTO
            BookingProcessedRequest bookingProcessedRequest = _mapper.Map<BookingProcessedRequest>(bookingCreationRequest);

            foreach (var ticket in bookingProcessedRequest.Tickets)
            {
                var flight = await _flightService.GetFlightById(ticket.FlightId);

                if (flight == null)
                {
                    return ServiceResult<BookingResponse>.Failure($"No flight found with the id: {ticket.FlightId}.");
                }

                var flightClass = await _flightService.GetFlightClassById(ticket.FlightClassId);

                if (flightClass == null)
                {
                    return ServiceResult<BookingResponse>.Failure($"No flight class found with the id: {ticket.FlightClassId}.");
                }

                ticket.FlightClassName = flightClass.Name;
                ticket.TicketNumber = UniqueSequenceGenerator.GenerateUniqueString(4);
                ticket.FlightPrice = CalculateTicketPrice(flight, flightClass);

                
                _ticketAvailabilityChecker.AddAmountOfTicketsForFlightIdAndFlightClass(ticket.FlightId, ticket.FlightClassName);
                _ticketAvailabilityChecker.AddFlight(flight);
            }

            bool ticketsAreAvailable = _ticketAvailabilityChecker.CheckTicketAvailability();
            if (!ticketsAreAvailable)
            {
                return ServiceResult<BookingResponse>.Failure($"Some of the tickets requested are unavailable.");
            }

            bookingProcessedRequest.ConfirmationNumber = UniqueSequenceGenerator.GenerateUniqueString(4);
            bookingProcessedRequest.UserId = user.Id;
            var createdBooking = await _bookingRepository.CreateBooking(bookingProcessedRequest);
            await _emailService.SendBookingConfirmationMail(bookingProcessedRequest);
            var mappedBooking = _mapper.Map<BookingResponse>(createdBooking);
            return ServiceResult<BookingResponse>.Success(mappedBooking, "The booking was created successfully.");
        }
        private decimal CalculateTicketPrice(Flight flight, FlightClass flightClass)
        {
            decimal ticketPrice = flightClass.PriceMultiplier * flight.Price;
            return ticketPrice;
        }

        
    }
}