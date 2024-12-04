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
			

        }
    }
}
