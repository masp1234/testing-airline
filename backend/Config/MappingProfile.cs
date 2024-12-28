using AutoMapper;
using backend.Dtos;
using backend.Models;
using backend.Models.MongoDB;
using backend.Models.Neo4jModels;

namespace backend.Config
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
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

            // Mappings from Neo4j entities to the "shared" models
            CreateMap<Neo4jAirline, Airline>();
            CreateMap<Neo4jAirplane, Airplane>();
            CreateMap<Neo4jAirport, Airport>();
            CreateMap<User, Neo4jUser>().ReverseMap();
            CreateMap<Neo4jBooking, Booking>().ReverseMap();
            CreateMap<Neo4jTicket, Ticket>();
            CreateMap<Neo4jCity, City>();
            CreateMap<Neo4jState, State>();
            CreateMap<Neo4jPassenger, Passenger>();
            CreateMap<Neo4jFlightClass, FlightClass>();

            // Define custom mapping for Flight that includes related objects
            CreateMap<Neo4jFlight, Flight>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DeparturePortNavigation, 
                    opt => opt.MapFrom((src, dest, destMember, context) => 
                        context.Items.ContainsKey("DepartureAirport") 
                        ? context.Mapper.Map<Airport>(context.Items["DepartureAirport"]) 
                        : null))
                .ForMember(dest => dest.ArrivalPortNavigation, 
                    opt => opt.MapFrom((src, dest, destMember, context) =>
                        context.Items.ContainsKey("ArrivalAirport") 
                        ? context.Mapper.Map<Airport>(context.Items["ArrivalAirport"]): null))
                .ForMember(dest => dest.FlightsAirline, 
                    opt => opt.MapFrom((src, dest, destMember, context) => 
                        context.Items.ContainsKey("Airline") 
                        ? context.Mapper.Map<Airline>(context.Items["Airline"]): null))
                .ForMember(dest => dest.FlightsAirplane, 
                    opt => opt.MapFrom((src, dest, destMember, context) => 
                        context.Items.ContainsKey("Airplane") 
                        ? context.Mapper.Map<Airplane>(context.Items["Airplane"]): null));

            // Mappings from MongoDB entities to the "shared" models
            CreateMap<AirlineMongo, Airline>();
            CreateMap<AirportMongo, Airport>();
            CreateMap<AirplaneMongo, Airplane>();

            CreateMap<AirlineSnapshot, Airline>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<AirplaneSnapshot, Airplane>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.EconomyClassSeats, opt => opt.MapFrom(src => src.EconomyClassSeats))
               .ForMember(dest => dest.BusinessClassSeats, opt => opt.MapFrom(src => src.BusinessClassSeats))
               .ForMember(dest => dest.FirstClassSeats, opt => opt.MapFrom(src => src.FirstClassSeats));

            // Additional mapping for AirportSnapshot -> Airport
            CreateMap<AirportSnapshot, Airport>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.City.Id))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City));

            CreateMap<CitySnapshot, City>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.State.Id))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State));

            CreateMap<StateSnapshot, State>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code));

            CreateMap<FlightMongo, Flight>()
                .ForMember(dest => dest.FlightsAirlineId, opt => opt.MapFrom(src => src.FlightsAirline.Id))
                .ForMember(dest => dest.FlightsAirplaneId, opt => opt.MapFrom(src => src.FlightsAirplane.Id))
                .ForMember(dest => dest.ArrivalPort, opt => opt.MapFrom(src => src.ArrivalPort.Id))
                .ForMember(dest => dest.DeparturePort, opt => opt.MapFrom(src => src.DeparturePort.Id))
                .ForMember(dest => dest.ArrivalPortNavigation, opt => opt.MapFrom(src => src.ArrivalPort))
                .ForMember(dest => dest.DeparturePortNavigation, opt => opt.MapFrom(src => src.DeparturePort));

            CreateMap<User, UserMongo>();
            CreateMap<UserMongo, User>();

            CreateMap<FlightClassMongo, FlightClass>();

            // Mapping for BookingMongo and embedded documents and snapshots
            CreateMap<FlightSnapShot, Flight>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FlightCode, opt => opt.MapFrom(src => src.FlightCode))
                .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => src.DepartureTime))
                .ForMember(dest => dest.CompletionTime, opt => opt.MapFrom(src => src.CompletionTime))
                .ForMember(dest => dest.TravelTime, opt => opt.MapFrom(src => src.TravelTime))
                .ForMember(dest => dest.Kilometers, opt => opt.MapFrom(src => src.Kilometers))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.EconomyClassSeatsAvailable, opt => opt.MapFrom(src => src.EconomyClassSeatsAvailable))
                .ForMember(dest => dest.BusinessClassSeatsAvailable, opt => opt.MapFrom(src => src.BusinessClassSeatsAvailable))
                .ForMember(dest => dest.FirstClassSeatsAvailable, opt => opt.MapFrom(src => src.FirstClassSeatsAvailable))
                .ForMember(dest => dest.ArrivalPortNavigation, opt => opt.MapFrom(src => src.ArrivalPort))
                .ForMember(dest => dest.DeparturePortNavigation, opt => opt.MapFrom(src => src.DeparturePort))
                .ForMember(dest => dest.FlightsAirline, opt => opt.MapFrom(src => src.FlightsAirline))
                .ForMember(dest => dest.FlightsAirplane, opt => opt.MapFrom(src => src.FlightsAirplane))
                .ForMember(dest => dest.FlightsAirplaneId, opt => opt.MapFrom(src => src.FlightsAirplane.Id))
                .ForMember(dest => dest.FlightsAirlineId, opt => opt.MapFrom(src => src.FlightsAirline.Id))
                .ForMember(dest => dest.DeparturePort, opt => opt.MapFrom(src => src.DeparturePort.Id))
                .ForMember(dest => dest.ArrivalPort, opt => opt.MapFrom(src => src.ArrivalPort.Id));

            CreateMap<UserSnapshot, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

            CreateMap<TicketEmbedded, Ticket>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.TicketNumber, opt => opt.MapFrom(src => src.TicketNumber))
                .ForMember(dest => dest.Flight, opt => opt.MapFrom(src => src.Flight))
                .ForMember(dest => dest.Passenger, opt => opt.MapFrom(src => src.Passenger))
                .ForMember(dest => dest.FlightClass, opt => opt.MapFrom(src => src.FlightClass))
                .ForMember(dest => dest.FlightId, opt => opt.MapFrom(src => src.Flight.Id))
                .ForMember(dest => dest.PassengerId, opt => opt.MapFrom(src => src.Passenger.Id))
                .ForMember(dest => dest.FlightClassId, opt => opt.MapFrom(src => src.FlightClass.Id));

            CreateMap<PassengerEmbedded, Passenger>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<FlightClassSnapshot, FlightClass>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.PriceMultiplier, opt => opt.MapFrom(src => src.PriceMultiplier));

            CreateMap<BookingMongo, Booking>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.ConfirmationNumber, opt => opt.MapFrom(src => src.ConfirmationNumber))
              .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
              .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
              .ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Tickets));

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
        }
    }
}