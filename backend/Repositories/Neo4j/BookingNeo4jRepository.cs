using Neo4jClient;
using backend.Models.Neo4jModels;
using AutoMapper;
using backend.Models;
using backend.Dtos;
using MongoDB.Bson;
using Neo4jClient.Cypher;
using Neo4j.Driver;
using backend.Utils;

namespace backend.Repositories.Neo4j;
public class BookingNeo4jRepository(IGraphClient graphClient, IMapper mapper): IBookingRepository
{
    private readonly IGraphClient _graphClient = graphClient;
    private readonly IMapper _mapper = mapper;

    private ICypherFluentQuery ApplyCommonRelationships(ICypherFluentQuery query)
    {
        return query
            .Match("(ticket:Ticket)-[:ASSOCIATED_WITH]->(booking:Booking)")
            .Match("(ticket:Ticket)-[:ISSUED_TO]->(passenger:Passenger)")
            .Match("(ticket:Ticket)-[:BOOKED_FOR]->(flight:Flight)")
            .Match("(flight:Flight)-[:DEPARTS_FROM]->(departureAirport:Airport)")
            .Match("(flight:Flight)-[:ARRIVES_AT]->(arrivalAirport:Airport)")
            .Match("(ticket:Ticket)-[:IN_CLASS]->(flightClass:FlightClass)");
    }
    

    public async Task<List<Booking>> GetBookingsByUserId(long id)
    {
        
        var query = await ApplyCommonRelationships( _graphClient.Cypher
            .Match("(booking:Booking)")
            .Where((Neo4jBooking booking) => booking.UserId == id))
            .Return((booking, ticket, passenger, flight, flightClass, departureAirport, arrivalAirport) => new
            {
                Booking = booking.As<Neo4jBooking>(),
                Ticket = ticket.CollectAs<Neo4jTicket>(),
                Passenger = passenger.As<Neo4jPassenger>(),
                Flight = flight.As<Neo4jFlight>(),
                FlightClass = flightClass.As<Neo4jFlightClass>(),
                DepartureAirport = departureAirport.As<Neo4jAirport>(),
                ArrivalAirport = arrivalAirport.As<Neo4jAirport>(),

            })
            .ResultsAsync;
        var bookings =  query
        .Select(result => new Booking
        {
            Id = result.Booking.Id,
            ConfirmationNumber = result.Booking.ConfirmationNumber,
            UserId = result.Booking.UserId,
            Tickets = result.Ticket.Select(ticket => new Ticket
            {
                Id = ticket.Id,
                Price = ticket.Price,
                TicketNumber = ticket.TicketNumber,
                PassengerId = ticket.PassengerId,
                FlightId = ticket.FlightId,
                TicketsBookingId = ticket.TicketsBookingId,
                FlightClassId = ticket.FlightClassId,
                Passenger = _mapper.Map<Passenger>(result.Passenger),
                Flight = _mapper.Map<Flight>(result.Flight, opt => 
                {
                    opt.Items["DepartureAirport"] = result.DepartureAirport;
                    opt.Items["ArrivalAirport"] = result.ArrivalAirport;
                }
                ),
                FlightClass = _mapper.Map<FlightClass>(result.FlightClass)

            }).ToList()
        })
        .ToList();
        return _mapper.Map<List<Booking>>(bookings);
    }

   public async Task<Booking> CreateBooking(BookingProcessedRequest request)
    {
        try
        {
            // Create the booking node
            var bookingId = UniqueSequenceGenerator.GenerateUniqueLongIdToNeo4j();
            await _graphClient.Cypher
                .Match("(user:User)")
                .Where((Neo4jUser user) => user.Id == request.UserId)
                .Create("(booking:Booking {id: $id, confirmation_number: $confirmationNumber, user_id: $userId})")
                .Create("(booking)-[:MADE_BY]->(user)")
                .WithParams(new
                {
                    id = bookingId,
                    confirmationNumber = request.ConfirmationNumber,
                    userId = request.UserId
                })
                
                .ExecuteWithoutResultsAsync();

            var createdBooking = new Booking
            {
                Id = bookingId,
                ConfirmationNumber = request.ConfirmationNumber,
                UserId = request.UserId
            };

            foreach (var ticket in request.Tickets)
            {

                // Create the passenger node
                var passengerId = UniqueSequenceGenerator.GenerateUniqueLongIdToNeo4j();
                await _graphClient.Cypher
                    .Create("(passenger:Passenger {id: $id, first_name: $firstName, last_name: $lastName, email: $email})")
                    .WithParams(new
                    {
                        id = passengerId,
                        firstName = ticket.Passenger.FirstName,
                        lastName = ticket.Passenger.LastName,
                        email = ticket.Passenger.Email
                    })
                    .ExecuteWithoutResultsAsync();

                // Create the ticket node and relationships
                var generatedTicketId = UniqueSequenceGenerator.GenerateUniqueLongIdToNeo4j();
                await _graphClient.Cypher
                    .Match("(booking:Booking)", "(passenger:Passenger)", "(flightClass: FlightClass)")
                    .Where((Neo4jBooking booking) => booking.Id == bookingId)
                    .AndWhere((Neo4jPassenger passenger) => passenger.Id == passengerId)
                    .AndWhere((Neo4jFlightClass flightClass) => flightClass.Name == ticket.FlightClassName)
                    .Merge("(flight:Flight {id: $flightId})")
                    .Create("(ticket:Ticket {id: $ticketId, flight_class_id: $flightClassId, price: $price, ticket_number: $ticketNumber})")
                    .WithParams(new
                    {
                        flightId = ticket.FlightId,
                        ticketId = generatedTicketId,
                        flightClassId = ticket.FlightClassId,
                        price = ticket.FlightPrice,
                        ticketNumber = ticket.TicketNumber
                    })
                    .Create("(ticket)-[:ISSUED_TO]->(passenger)")
                    .Create("(ticket)-[:ASSOCIATED_WITH]->(booking)")
                    .Create("(ticket)-[:BOOKED_FOR]->(flight)")
                    .Create("(ticket)-[:IN_CLASS]->(flightClass)")
                    .ExecuteWithoutResultsAsync();

                // Decrement seat availability and update flight version

                string flightClassName = "";

                var flightQuery = await _graphClient.Cypher
                    .Match("(flight:Flight)")
                    .Where((Neo4jFlight flight) => flight.Id == ticket.FlightId)
                    .Return((flight) => new
                    {
                        Flight = flight.As<Neo4jFlight>(),
                    })
                    .ResultsAsync;
                var flight = flightQuery.SingleOrDefault();
                
                switch (ticket.FlightClassName)
                {
                    case FlightClassName.EconomyClass:
                        if (flight?.Flight.EconomyClassSeatsAvailable > 0)
                            flightClassName = "flight.economy_class_seats_available";
                        else
                            throw new InvalidOperationException("No Economy class seats available.");
                        break;

                    case FlightClassName.BusinessClass:
                        if (flight?.Flight.BusinessClassSeatsAvailable > 0)
                            flightClassName = "flight.business_class_seats_available";
                        else
                            throw new InvalidOperationException("No Business class seats available.");
                        break;

                    case FlightClassName.FirstClass:
                        if (flight?.Flight.EconomyClassSeatsAvailable > 0)
                            flightClassName = "flight.first_class_seats_available";
                        else
                            throw new InvalidOperationException("No First Class seats available.");
                        break;

                    default:
                        throw new ArgumentException($"Invalid flight class: {flightClassName}");
                }
                
                var setFlightClass = flightClassName+ "="+flightClassName+" -1";
                await _graphClient.Cypher
                    .Match("(flight:Flight {id: $flightId})")
                    .Set(setFlightClass)
                    .Set("flight.version = flight.version + 1")
                    .WithParams(new
                    {
                        flightId = ticket.FlightId,                         
                    })
                    .ExecuteWithoutResultsAsync();



                }

            return createdBooking;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while creating the booking.", ex);
        }
    }



}