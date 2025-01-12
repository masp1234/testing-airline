// To set the test data run the script: mongosh --file init-mongo.js MONGODB_CONNECTION_STRING

db = connect(MONGODB_CONNECTION_STRING);

db = db.getSiblingDB('mydatabase'); 


db.airlines.insertMany([
    { name: "Delta Airlines" },
    { name: "United Airlines" },
    { name: "American Airlines" },
    { name: "Southwest Airlines" },
    { name: "JetBlue Airways" },
    { name: "Alaska Airlines" },
    { name: "Spirit Airlines" },
    { name: "Frontier Airlines" },
    { name: "Hawaiian Airlines" },
    { name: "Allegiant Air" }
]);

db.airplanes.insertMany([
    { name: "Boeing 737", airplanes_airline_id: 1, economy_class_seats: 140, business_class_seats: 40, first_class_seats: 5 },
    { name: "Airbus A320", airplanes_airline_id: 2, economy_class_seats: 120, business_class_seats: 22, first_class_seats: 20 },
    { name: "Boeing 777", airplanes_airline_id: 3, economy_class_seats: 120, business_class_seats: 12, first_class_seats: 20 },
    { name: "Embraer E175", airplanes_airline_id: 4, economy_class_seats: 4, business_class_seats: 2, first_class_seats: 1 },
    { name: "Airbus A321", airplanes_airline_id: 5, economy_class_seats: 222, business_class_seats: 0, first_class_seats: 0 }
]);

db.airports.insertMany([
    { name: "Los Angeles International Airport", code: "LAX", city_id: 1 },
    { name: "John F. Kennedy International Airport", code: "JFK", city_id: 2 },
    { name: "George Bush Intercontinental Airport", code: "IAH", city_id: 3 },
    { name: "Miami International Airport", code: "MIA", city_id: 4 },
    { name: "O'Hare International Airport", code: "ORD", city_id: 5 }
]);

db.bookings.insertMany([
    { confirmation_number: "ABC123", user_id: 2 },
    { confirmation_number: "DEF456", user_id: 2 },
    { confirmation_number: "GHI789", user_id: 3 },
    { confirmation_number: "JKL012", user_id: 3 },
    { confirmation_number: "MNO345", user_id: 4 }
]);

db.flights.insertMany([
    {
      flight_code: "DL096",
      departure_port: 1,
      arrival_port: 2,
      departure_time: ISODate("2024-12-24T02:30:00Z"),
      travel_time: 360,
      completion_time: ISODate("2024-12-24T10:30:00Z"),
      price: 199.99,
      kilometers: 450,
      economy_class_seats_available: 150,
      business_class_seats_available: 20,
      first_class_seats_available: 5,
      flights_airline_id: 1,
      flights_airplane_id: 5,
      idempotency_key: "TEST_DL096",
      created_by: "test@gmail.com",
      updated_by: "test@gmail.com"
    },
    {
      flight_code: "DL097",
      departure_port: 1,
      arrival_port: 2,
      departure_time: ISODate("2024-12-24T07:30:00Z"),
      travel_time: 360,
      completion_time: ISODate("2024-12-24T15:30:00Z"),
      price: 199.99,
      kilometers: 450,
      economy_class_seats_available: 150,
      business_class_seats_available: 20,
      first_class_seats_available: 5,
      flights_airline_id: 1,
      flights_airplane_id: 10,
      idempotency_key: "TEST_DL097",
      created_by: "test@gmail.com",
      updated_by: "test@gmail.com"
    }
]);

db.users.insertMany([
    { email: "admin@example.com", password: "AQAAAAIAAYagAAAAECrPK/OaPJ1TtlTl0to+E6f86of8ocaDjTNDunN9fPPlIpFUd787gRpL2lusp8srkg==", role: "Admin" },
    { email: "customer@example.com", password: "AQAAAAIAAYagAAAAEJvAdN3g69LF6cuKWK/xIHyUyz1qtNoVCMgKIlSd5oTPwk+7/A+qEAcxQJ2B+FvghQ==", role: "Customer" }
]);
