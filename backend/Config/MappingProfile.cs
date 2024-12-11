using AutoMapper;
using backend.Dtos;
using backend.Models;

namespace backend.Config
{
	public class MappingProfile: Profile
	{
		public MappingProfile() {
			CreateMap<User, UserResponse>();

			CreateMap<FlightCreationRequest, Flight>()
			   .ForMember(dest => dest.FlightsAirlineId, opt => opt.MapFrom(src => src.AirlineId))
			   .ForMember(dest => dest.FlightsAirplaneId, opt => opt.MapFrom(src => src.AirplaneId))
			   .ForMember(dest => dest.DeparturePort, opt => opt.MapFrom(src => src.DepartureAirportId))
			   .ForMember(dest => dest.ArrivalPort, opt => opt.MapFrom(src => src.ArrivalAirportId))
			   .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => src.DepartureDateTime));
			
			CreateMap<Flight, FlightResponse>();

			CreateMap<User, JwtRequest>();

			CreateMap<Airport, AirportResponse>();

			CreateMap<Airplane, AirplaneResponse>();
			
			CreateMap<Airline, AirlineResponse>();

			CreateMap<BookingCreationRequest, BookingProcessedRequest>();

			CreateMap<TicketCreationRequest, TicketProcessedRequest>();

			CreateMap<Booking, BookingResponse>();
			
			CreateMap<Ticket, TicketResponse>()
				.ForMember(dest => dest.FlightCode, opt => opt.MapFrom(src => src.Flight.FlightCode))
				.ForMember(dest => dest.DeparturePortName, opt => opt.MapFrom(src => src.Flight.DeparturePortNavigation.Name))
				.ForMember(dest => dest.ArrivalPortName, opt => opt.MapFrom(src => src.Flight.ArrivalPortNavigation.Name))
				.ForMember(dest => dest.FlightClassName, opt => opt.MapFrom(src => src.FlightClass.Name))
				.ForMember(dest => dest.FlightTravelTime, opt => opt.MapFrom(src => src.Flight.TravelTime))
				.ForMember(dest => dest.FlightDepartureTime, opt => opt.MapFrom(src => src.Flight.DepartureTime))
				.ForMember(dest => dest.FlightCompletionTime, opt => opt.MapFrom(src => src.Flight.CompletionTime))
				.ForMember(dest => dest.PassengerFirstName, opt => opt.MapFrom(src => src.Passenger.FirstName))
				.ForMember(dest => dest.PassengerLastName, opt => opt.MapFrom(src => src.Passenger.LastName))
				.ForMember(dest => dest.PassengerEmail, opt => opt.MapFrom(src => src.Passenger.Email));

			CreateMap<Booking, BookingResponse>()
				.ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Tickets));
		}
	}
}
