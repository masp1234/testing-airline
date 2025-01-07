// Create Airline Nodes
CREATE (a1:Airline {id: 1, name: 'Delta Airlines'})
CREATE (a2:Airline {id: 2, name: 'United Airlines'})

// Create Airplane Nodes
CREATE (airplane1:Airplane {id: 1, name: 'Boeing 737', economy_class_seats: 140, business_class_seats: 40, first_class_seats: 5})
CREATE (airplane2:Airplane {id: 2, name: 'Airbus A320', economy_class_seats: 120, business_class_seats: 22, first_class_seats: 20})

// Link Airplanes to Airlines
CREATE (airplane1)-[:BELONGS_TO]->(a1)
CREATE (airplane2)-[:BELONGS_TO]->(a2)

// Create State Nodes
CREATE (state1:State {id: 2, code: 'NY'})
CREATE (state2:State {id: 1, code: 'CA'})
CREATE (state7:State {id: 7, code: 'CO'})

// Create City Nodes and link to States
CREATE (city1:City {id: 2, name: 'New York'})-[:LOCATED_IN]->(state2)
CREATE (city2:City {id: 1, name: 'Los Angeles'})-[:LOCATED_IN]->(state1)
CREATE (city7:City {id: 7, name: 'Denver'})-[:LOCATED_IN]->(state7)

// Create Airport Nodes and link to Cities
CREATE (airport1:Airport {id: 1, name: 'Los Angeles International Airport', code: 'LAX'})-[:LOCATED_IN]->(city1)
CREATE (airport2:Airport {id: 2, name: 'John F. Kennedy International Airport', code: 'JFK'})-[:LOCATED_IN]->(city2)
CREATE (airport7:Airport {id: 7, name: 'Denver International Airport', code: 'DEN'})-[:LOCATED_IN]->(city7)

// Create User Nodes
CREATE (user1:User {id: 1, email: 'customer@example.com', password: 'AQAAAAIAAYagAAAAECrPK/OaPJ1TtlTl0to+E6f86of8ocaDjTNDunN9fPPlIpFUd787gRpL2lusp8srkg==', role: 'Customer'})
CREATE (user2:User {id: 2, email: 'admin@example.com', password: 'AQAAAAIAAYagAAAAECrPK/OaPJ1TtlTl0to+E6f86of8ocaDjTNDunN9fPPlIpFUd787gRpL2lusp8srkg==', role: 'Admin'})

// Create Booking Nodes
CREATE (booking1:Booking {id: 1, confirmation_number: 'ABC123', user_id: 1})
CREATE (booking2:Booking {id: 2, confirmation_number: 'DEF456', user_id: 2})

// Link Bookings to Users
CREATE (booking1)-[:MADE_BY]->(user1)
CREATE (booking2)-[:MADE_BY]->(user2)

// Create Flight Class Nodes
CREATE (economyClass:FlightClass {id: 1, name: 'EconomyClass', price_multiplier: 1.0})
CREATE (businessClass:FlightClass {id: 2, name: 'BusinessClass', price_multiplier: 1.5})
CREATE (firstClass:FlightClass {id: 3, name: 'FirstClass', price_multiplier: 2.0})

// Create Flight Nodes
CREATE (flight1:Flight {id: 1, flight_code: 'FL123', departure_port: 1, arrival_port: 2, departure_time: '2024-12-15T10:00:00', completion_time: '2024-12-15T12:00:00', travel_time: 120, price: 200.00, kilometers: 500, economy_class_seats_available: 80, business_class_seats_available: 15, first_class_seats_available: 5, flights_airline_id: 1, flights_airplane_id: 1, idempotency_key: 'key123', created_by: 'admin'})

CREATE (flight2:Flight {id: 2, flight_code: 'FL456', departure_port: 2, arrival_port: 1, departure_time: '2024-12-15T14:00:00', completion_time: '2024-12-15T16:00:00', travel_time: 120, price: 250.00, kilometers: 600, economy_class_seats_available: 100, business_class_seats_available: 20, first_class_seats_available: 10, flights_airline_id: 2, flights_airplane_id: 2, idempotency_key: 'key123', created_by: 'admin'})



// Link Flights to Airports (Departure and Arrival)
CREATE (flight1)-[:DEPARTS_FROM]->(airport1)
CREATE (flight1)-[:ARRIVES_AT]->(airport2)
CREATE (flight2)-[:DEPARTS_FROM]->(airport2)
CREATE (flight2)-[:ARRIVES_AT]->(airport1)

// Link Flights to Airlines and Airplanes
CREATE (flight1)-[:OPERATED_BY]->(a1)
CREATE (flight1)-[:FLIES_ON]->(airplane1)
CREATE (flight2)-[:OPERATED_BY]->(a2)
CREATE (flight2)-[:FLIES_ON]->(airplane2)

// Create Passenger Nodes
CREATE (passenger1:Passenger {id: 1, first_name: 'John', last_name: 'Doe', email: 'john.doe@example.com'})
CREATE (passenger2:Passenger {id: 2, first_name: 'Jane', last_name: 'Smith', email: 'jane.smith@example.com'})

// Create Ticket Nodes
CREATE (ticket1:Ticket {id: 1, price: 200.00, ticket_number: 'TICK123'})
CREATE (ticket2:Ticket {id: 2, price: 250.00, ticket_number: 'TICK456'})

// Link Tickets to Flights, Passengers, and Bookings
CREATE (ticket1)-[:BOOKED_FOR]->(flight1)
CREATE (ticket1)-[:ISSUED_TO]->(passenger1)
CREATE (ticket1)-[:ASSOCIATED_WITH]->(booking1)
CREATE (ticket1)-[:IN_CLASS]->(economyClass)

CREATE (ticket2)-[:BOOKED_FOR]->(flight2)
CREATE (ticket2)-[:ISSUED_TO]->(passenger2)
CREATE (ticket2)-[:ASSOCIATED_WITH]->(booking2)
CREATE (ticket2)-[:IN_CLASS]->(businessClass)

// OPTIONAL: For procedures like 'CheckAndInsertFlight', Neo4j doesn't support procedural SQL, but you can model the logic using constraints or Cypher queries. You could, for example, check if a flight already exists before adding a new one with a unique code or departure/arrival time overlap.
