using Neo4jClient;
using backend.Models.Neo4jModels;
using AutoMapper;
using backend.Models;
using backend.Utils;
using Neo4jClient.Cypher;
using MongoDB.Bson;

namespace backend.Repositories.Neo4j;
public class FlightNeo4jRepository(IGraphClient graphClient, IMapper mapper): IFlightRepository
{
    private readonly IGraphClient _graphClient = graphClient;
    private readonly IMapper _mapper = mapper;

    private ICypherFluentQuery ApplyCommonRelationships(ICypherFluentQuery query)
    {
        return query
            .Match("(flight:Flight)-[:DEPARTS_FROM]->(departureAirport:Airport)")
            .Match("(flight:Flight)-[:ARRIVES_AT]->(arrivalAirport:Airport)")
            .Match("(flight:Flight)-[:OPERATED_BY]->(airline:Airline)")
            .Match("(ticket:Ticket)-[:ISSUED_TO]->(passenger:Passenger)")
            .Match("(ticket:Ticket)-[:BOOKED_FOR]->(flight:Flight)")
            .Match("(ticket:Ticket)-[:IN_CLASS]->(flightClass:FlightClass)")
            .Match("(flight:Flight)-[:FLIES_ON]->(airplane:Airplane)");
    }
    public async Task<List<Flight>> GetAll()
    {
        var query = await _graphClient.Cypher
            .Match("(f:Flight)")
            .Match("(f:Flight)-[:DEPARTS_FROM]->(departureAirport:Airport)")
            .Match("(f)-[:ARRIVES_AT]->(arrivalAirport:Airport)")
            .Match("(f)-[:OPERATED_BY]->(airline:Airline)")
            .Match("(f)-[:FLIES_ON]->(airplane:Airplane)") 
            .Return((f, departureAirport, arrivalAirport, airline, airplane) => new
            {
                Flight = f.As<Neo4jFlight>(),
                DepartureAirport = departureAirport.As<Neo4jAirport>(),
                ArrivalAirport = arrivalAirport.As<Neo4jAirport>(),
                Airline = airline.As<Neo4jAirline>(),
                Airplane = airplane.As<Neo4jAirplane>()
            })
            .ResultsAsync;

        var flights = query.Select(result =>
            _mapper.Map<Flight>(result.Flight, opt =>
            {
                opt.Items["DepartureAirport"] = result.DepartureAirport;
                opt.Items["ArrivalAirport"] = result.ArrivalAirport;
                opt.Items["Airline"] = result.Airline;
                opt.Items["Airplane"] = result.Airplane;
            })
            ).ToList();

        return flights;;  
    }
    

    public async Task<Flight> Create(Flight flight)
    {
        if (flight == null)
        {
            throw new ArgumentNullException(nameof(flight));
        }
        // Check for overlapping flights
        var overlapCount = await _graphClient.Cypher
        .Match("(flight:Flight)")
        .Where("flight.AirplaneId = $airplaneId")
        .AndWhere("flight.departure_time < $completionTime AND flight.completion_time > $departureTime")
        .WithParams(new
        {
            airplaneId = flight.FlightsAirplaneId,
            departureTime = flight.DepartureTime,
            completionTime = flight.CompletionTime
        })
        .Return(flight => flight.Count())
        .ResultsAsync;

        var singleOverlapCount = overlapCount.SingleOrDefault();

        if (singleOverlapCount > 0)
        {
            throw new InvalidOperationException("Overlap detected with existing flight schedule.");
        }


        // Generate a unique ID for the flight
        flight.Id = UniqueSequenceGenerator.GenerateUniqueLongIdToNeo4j();

        await _graphClient.Cypher
            .Match("(departureAirport:Airport)", "(arrivalAirport:Airport)", "(airline:Airline)", "(airplane:Airplane)")
            .Where((Neo4jAirport departureAirport) => departureAirport.Id == flight.DeparturePort)
            .AndWhere((Neo4jAirport arrivalAirport) => arrivalAirport.Id == flight.ArrivalPort)
            .AndWhere((Neo4jAirline airline) => airline.Id == flight.FlightsAirlineId)
            .AndWhere((Neo4jAirline airplane) => airplane.Id == flight.FlightsAirplaneId)
            .Create("(flight:Flight {id: $id, flight_code: $flightCode, flights_airplane_id: $airplaneId, departure_time: $departureTime, completion_time: $completionTime, departure_port: $departurePort, arrival_port: $arrivalPort, travel_time: $travelTime, price: $price, kilometers: $kilometers, economy_class_seats_available: $economySeats, business_class_seats_available: $businessSeats, first_class_seats_available: $firstClassSeats, flights_airline_id: $airlineId, idempotency_key: $idempotencyKey, created_by: $createdBy})")
            .Create("(flight)-[:DEPARTS_FROM]->(departureAirport)")
            .Create("(flight)-[:ARRIVES_AT]->(arrivalAirport)")
            .Create("(flight)-[:OPERATED_BY]->(airline)")
            .Create("(flight)-[:FLIES_ON]->(airplane)")
            .WithParams(new
            {
                id = flight.Id,
                flightCode = flight.FlightCode,
                airplaneId = flight.FlightsAirplaneId,
                departureTime = flight.DepartureTime,
                completionTime = flight.CompletionTime,
                departurePort = flight.DeparturePort,
                arrivalPort = flight.ArrivalPort,
                travelTime = flight.TravelTime,
                price = flight.Price,
                kilometers = flight.Kilometers,
                economySeats = flight.EconomyClassSeatsAvailable,
                businessSeats = flight.BusinessClassSeatsAvailable,
                firstClassSeats = flight.FirstClassSeatsAvailable,
                airlineId = flight.FlightsAirlineId,
                idempotencyKey = flight.IdempotencyKey,
                createdBy = flight.CreatedBy
            })
            .ExecuteWithoutResultsAsync();

        return flight;
        
    }


    public async Task<Flight> Delete(long flightId, string deletedBy)
    {
        // Start a transaction
        await _graphClient.Cypher
            .Match("(flight:Flight {id: $id})")
            .Match("(ticket:Ticket)-[:BOOKED_FOR]->(flight:Flight)")
            .Match("(ticket:Ticket)-[:ISSUED_TO]->(passenger:Passenger)")
            .Match("(ticket:Ticket)-[:ASSOCIATED_WITH]->(booking:Booking)")
            .OptionalMatch("(flight)-[r1]-()") // Match any remaining relationships for the flight
            .OptionalMatch("(ticket)-[r2]-()") // Match any remaining relationships for the ticket
            .OptionalMatch("(passenger)-[r3]-()")//Match any remaining relationships for the passenger
            .OptionalMatch("(booking)-[r4]-()") // Match any remaining relationships for the booking
            .With("flight, ticket, passenger, booking, r1, r2, r3, r4, $userEmail AS deletedBy")
            .Set("flight.deletedBy = deletedBy") // Log who deleted the flight
            .Delete("r1, r2, r3, r4, ticket, passenger, booking, flight") // First delete relationships, then nodes
            .WithParams(new
            {
                id = flightId,
                userEmail = deletedBy
            })
            .ExecuteWithoutResultsAsync();

        // Return a dummy Flight object with the ID (to indicate deletion was successful)
        return new Flight { Id = flightId };
    }


    

    public async Task<Flight?> GetFlightById(long id)
    {
        var query = await _graphClient.Cypher
            .Match("(f:Flight)-[:DEPARTS_FROM]->(departureAirport:Airport)")
            .Match("(f)-[:ARRIVES_AT]->(arrivalAirport:Airport)")
            .Match("(f)-[:OPERATED_BY]->(airline:Airline)")
            .Match("(f)-[:FLIES_ON]->(airplane:Airplane)")
            .Where((Neo4jFlight f) => f.Id == id)
            .Return((f, departureAirport, arrivalAirport, airline, airplane) => new
            {
                Flight = f.As<Neo4jFlight>(),
                DepartureAirport = departureAirport.As<Neo4jAirport>(),
                ArrivalAirport = arrivalAirport.As<Neo4jAirport>(),
                Airline = airline.As<Neo4jAirline>(),
                Airplane = airplane.As<Neo4jAirplane>()
            })
            .ResultsAsync;


            var flight = query.Select(result =>
                _mapper.Map<Flight>(result.Flight, opt =>
                {
                    opt.Items["DepartureAirport"] = result.DepartureAirport;
                    opt.Items["ArrivalAirport"] = result.ArrivalAirport;
                    opt.Items["Airline"] = result.Airline;
                    opt.Items["Airplane"] = result.Airplane;
                })
                )
                .SingleOrDefault();


        return flight == null? null : _mapper.Map<Flight>(flight);
    }

    public async Task<Flight?> GetFlightByIdempotencyKey(string idempotencyKey)
    {
        var query = await _graphClient.Cypher
            .Match("(f:Flight)-[:DEPARTS_FROM]->(departureAirport:Airport)")
            .Match("(f)-[:ARRIVES_AT]->(arrivalAirport:Airport)")
            .Match("(f)-[:OPERATED_BY]->(airline:Airline)")
            .Match("(f)-[:FLIES_ON]->(airplane:Airplane)")
            .Where((Neo4jFlight f) => f.IdempotencyKey == idempotencyKey)
            .Return((f, departureAirport, arrivalAirport, airline, airplane) => new
            {
                Flight = f.As<Neo4jFlight>(),
                DepartureAirport = departureAirport.As<Neo4jAirport>(),
                ArrivalAirport = arrivalAirport.As<Neo4jAirport>(),
                Airline = airline.As<Neo4jAirline>(),
                Airplane = airplane.As<Neo4jAirplane>()
            })
            .ResultsAsync;

            var flight = query.Select(result =>
                _mapper.Map<Flight>(result.Flight, opt =>
                {
                    opt.Items["DepartureAirport"] = result.DepartureAirport;
                    opt.Items["ArrivalAirport"] = result.ArrivalAirport;
                    opt.Items["Airline"] = result.Airline;
                    opt.Items["Airplane"] = result.Airplane;
                })
                )
                .SingleOrDefault();

        return flight == null? null : _mapper.Map<Flight>(flight);
    }

    public async Task<FlightClass?> GetFlightClassById(long id)
    {
        var query = await _graphClient.Cypher
            .Match("(flightClass:FlightClass)")  
            .Where((Neo4jFlightClass flightClass) => flightClass.Id == id)
            .Return(flightClass => flightClass.As<Neo4jFlightClass>())  
            .ResultsAsync;

            var flightClass = query.SingleOrDefault();
        return flightClass == null? null : _mapper.Map<FlightClass>(flightClass);
    }

    public async Task<List<Flight>> GetFlightsByAirplaneId(long airplaneId)
    {
        var query = await ApplyCommonRelationships(_graphClient.Cypher
            .Match("(flight:Flight)")
            .Where((Neo4jFlight flight) => flight.FlightsAirplaneId == airplaneId))
            .Return((flight, departureAirport, arrivalAirport, airline, airplane) => new
            {
                Flight = flight.As<Neo4jFlight>(),
                DepartureAirport = departureAirport.As<Neo4jAirport>(),
                ArrivalAirport = arrivalAirport.As<Neo4jAirport>(),
                Airline = airline.As<Neo4jAirline>(),
                Airplane = airplane.As<Neo4jAirplane>()
            })
            .ResultsAsync;

            var flights = query.Select(result =>
                _mapper.Map<Flight>(result.Flight, opt =>
                {
                    opt.Items["DepartureAirport"] = result.DepartureAirport;
                    opt.Items["ArrivalAirport"] = result.ArrivalAirport;
                    opt.Items["Airline"] = result.Airline;
                    opt.Items["Airplane"] = result.Airplane;
                })
            )
            .ToList();
        
        return _mapper.Map<List<Flight>>(flights);
    }

    public async Task<List<Flight>> GetFlightsByAirplaneIdAndTimeInterval(Flight newflight)
    {
        var query = await _graphClient.Cypher
            .Match("(f:Flight)-[:DEPARTS_FROM]->(departureAirport:Airport)")
            .Match("(f)-[:ARRIVES_AT]->(arrivalAirport:Airport)")
            .Match("(f)-[:OPERATED_BY]->(airline:Airline)")
            .Match("(f)-[:FLIES_ON]->(airplane:Airplane)")
            .Where((Neo4jFlight f) => f.FlightsAirplaneId == newflight.FlightsAirplaneId
                    && f.DepartureTime < newflight.CompletionTime
                    && f.CompletionTime > newflight.DepartureTime)
            .Return((f, departureAirport, arrivalAirport, airline, airplane) => new
            {
                Flight = f.As<Neo4jFlight>(),
                DepartureAirport = departureAirport.As<Neo4jAirport>(),
                ArrivalAirport = arrivalAirport.As<Neo4jAirport>(),
                Airline = airline.As<Neo4jAirline>(),
                Airplane = airplane.As<Neo4jAirplane>()
            })
            .ResultsAsync;

            var flights = query.Select(result =>
                _mapper.Map<Flight>(result.Flight, opt =>
                {
                    opt.Items["DepartureAirport"] = result.DepartureAirport;
                    opt.Items["ArrivalAirport"] = result.ArrivalAirport;
                    opt.Items["Airline"] = result.Airline;
                    opt.Items["Airplane"] = result.Airplane;
                })
                )
                .ToList();

        return _mapper.Map<List<Flight>>(flights);
    }

    public async Task<List<Flight>> GetFlightsByDepartureDestinationAndDepartureDate(long departureAirportId, long destinationAirportId, DateOnly departureDate)
    {
        DateTime startOfDay = departureDate.ToDateTime(TimeOnly.MinValue);
        DateTime endOfDay = departureDate.ToDateTime(TimeOnly.MaxValue);

        var query = await _graphClient.Cypher
            .Match("(flight:Flight)-[:DEPARTS_FROM]->(departureAirport:Airport)")
            .Match("(flight)-[:ARRIVES_AT]->(arrivalAirport:Airport)")
            .Match("(flight)-[:OPERATED_BY]->(airline:Airline)")
            .Match("(flight)-[:FLIES_ON]->(airplane:Airplane)")
            .Where((Neo4jFlight flight) =>
                    flight.DeparturePort == departureAirportId &&
                    flight.ArrivalPort == destinationAirportId &&
                    flight.DepartureTime >= startOfDay &&
                    flight.DepartureTime <= endOfDay      
                )
            .Return((flight, departureAirport, arrivalAirport, airline, airplane) => new
            {
                Flight = flight.As<Neo4jFlight>(),
                DepartureAirport = departureAirport.As<Neo4jAirport>(),
                ArrivalAirport = arrivalAirport.As<Neo4jAirport>(),
                Airline = airline.As<Neo4jAirline>(),
                Airplane = airplane.As<Neo4jAirplane>()
            })
            .ResultsAsync;

            var flights = query.Select(result =>
                _mapper.Map<Flight>(result.Flight, opt =>
                {
                    opt.Items["DepartureAirport"] = result.DepartureAirport;
                    opt.Items["ArrivalAirport"] = result.ArrivalAirport;
                    opt.Items["Airline"] = result.Airline;
                    opt.Items["Airplane"] = result.Airplane;
                })
                )
                .ToList();
        
        return _mapper.Map<List<Flight>>(flights);
    }

    public async Task<Flight?> GetFlightWithRelationshipsById(long id)
    {
        var query = await _graphClient.Cypher
            .Match("(f:Flight)-[:DEPARTS_FROM]->(departureAirport:Airport)")
            .Match("(f)-[:ARRIVES_AT]->(arrivalAirport:Airport)")
            .Match("(f)-[:OPERATED_BY]->(airline:Airline)")
            .Match("(f)-[:FLIES_ON]->(airplane:Airplane)")
            .Where((Neo4jFlight f) => f.Id == id)
            .Return((f, departureAirport, arrivalAirport, airline, airplane) => new
            {
                Flight = f.As<Neo4jFlight>(),
                DepartureAirport = departureAirport.As<Neo4jAirport>(),
                ArrivalAirport = arrivalAirport.As<Neo4jAirport>(),
                Airline = airline.As<Neo4jAirline>(),
                Airplane = airplane.As<Neo4jAirplane>()
            })
            .ResultsAsync;

            var flight = query.Select(result =>
                _mapper.Map<Flight>(result.Flight, opt =>
                {
                    opt.Items["DepartureAirport"] = result.DepartureAirport;
                    opt.Items["ArrivalAirport"] = result.ArrivalAirport;
                    opt.Items["Airline"] = result.Airline;
                    opt.Items["Airplane"] = result.Airplane;
                })
                )
                .SingleOrDefault();

        return _mapper.Map<Flight>(flight);
    }

    public async Task<List<Ticket>> GetTicketsByFlightId(long flightId)
    {
       var query = await _graphClient.Cypher
        .Match("(ticket:Ticket)-[:ISSUED_TO]->(passenger:Passenger)")
        .Match("(ticket:Ticket)-[:BOOKED_FOR]->(flight:Flight)")
        .Match("(flight:Flight)-[:DEPARTS_FROM]->(departureAirport:Airport)")
        .Match("(flight:Flight)-[:ARRIVES_AT]->(arrivalAirport:Airport)")
        .Match("(ticket:Ticket)-[:IN_CLASS]->(flightClass:FlightClass)")
        .Where((Neo4jFlight flight) => flight.Id == flightId) // Filter by Flight ID
        .Return((ticket, passenger, flight, flightClass, departureAirport, arrivalAirport) => new
        {
            Ticket = ticket.As<Neo4jTicket>(),
            Passenger = passenger.As<Neo4jPassenger>(),
            Flight = flight.As<Neo4jFlight>(),
            FlightClass = flightClass.As<Neo4jFlightClass>(),
            DepartureAirport = departureAirport.As<Neo4jAirport>(),
            ArrivalAirport = arrivalAirport.As<Neo4jAirport>(),

        }) // Return the ticket nodes
        .ResultsAsync;

        var tickets = query.Select(res => new Ticket
            {
                Id = res.Ticket.Id,
                Price = res.Ticket.Price,
                TicketNumber = res.Ticket.TicketNumber,
                PassengerId = res.Ticket.PassengerId,
                FlightId = res.Ticket.FlightId,
                TicketsBookingId = res.Ticket.TicketsBookingId,
                FlightClassId = res.Ticket.FlightClassId,
                Passenger = _mapper.Map<Passenger>(res.Passenger),
                Flight = _mapper.Map<Flight>(res.Flight, opt => 
                {
                    opt.Items["DepartureAirport"] = res.DepartureAirport;
                    opt.Items["ArrivalAirport"] = res.ArrivalAirport;
                }
                ),
                FlightClass = _mapper.Map<FlightClass>(res.FlightClass)

            }).ToList();
        return _mapper.Map<List<Ticket>>(tickets); // Map to your domain model
    }


    public async Task<bool> UpdateFlight(Flight flightToUpdate)
{
    try
    {
        // Start the update process
        var overlappingFlights = await GetFlightsByAirplaneIdAndTimeInterval(flightToUpdate);

        // Check for overlapping flights
        if (overlappingFlights.Any(flight => flight.Id != flightToUpdate.Id))
        {
            throw new Exception("Update denied, there were overlapping flights");
        }

        // Update the flight
        await _graphClient.Cypher
            .Match("(flight:Flight {id: $flightId})")
            .Set("flight.departure_time = $departureTime, flight.completion_time = $completionTime") // Updates only the specified properties
            .WithParams(new
            {
                flightId = flightToUpdate.Id,
                departureTime = flightToUpdate.DepartureTime,
                completionTime = flightToUpdate.CompletionTime
            })
            .ExecuteWithoutResultsAsync();

        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating flight: {ex.Message}");
        return false;
    }
}
}