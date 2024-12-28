-- Sample data for `airlines`
INSERT INTO `airline_project`.`airlines` (`name`) VALUES
('Delta Airlines'),
('United Airlines'),
('American Airlines'),
('Southwest Airlines'),
('JetBlue Airways'),
('Alaska Airlines'),
('Spirit Airlines'),
('Frontier Airlines'),
('Hawaiian Airlines'),
('Allegiant Air');

-- Sample data for `airplanes`
INSERT INTO `airline_project`.`airplanes` (`name`, `airplanes_airline_id`, `economy_class_seats`, `business_class_seats`, `first_class_seats`) VALUES
('Boeing 737', 1, 140, 40, 5),
('Airbus A320', 2, 120, 22, 20),
('Boeing 777', 3, 120, 12, 20),
('Embraer E175', 4, 4, 2, 1),
('Airbus A321', 5, 222, 0, 0),
('Boeing 767', 6, 120, 40, 20),
('Airbus A319', 7, 120, 40, 20),
('Boeing 757', 8, 120, 40, 20),
('Boeing 787', 9, 120, 40, 20),
('Airbus A380', 10, 120, 40, 20);

-- Sample data for `states`
INSERT INTO `airline_project`.`states` (`code`) VALUES
('CA'),
('NY'),
('TX'),
('FL'),
('IL'),
('NV'),
('CO'),
('WA'),
('MA'),
('AZ');

-- Sample data for `cities`
INSERT INTO `airline_project`.`cities` (`name`, `state_id`) VALUES
('Los Angeles', 1),
('New York', 2),
('Houston', 3),
('Miami', 4),
('Chicago', 5),
('Las Vegas', 6),
('Denver', 7),
('Seattle', 8),
('Boston', 9),
('Phoenix', 10);

-- Sample data for `airports`
INSERT INTO `airline_project`.`airports` (`name`, `code`, `city_id`) VALUES
('Los Angeles International Airport', 'LAX', 1),
('John F. Kennedy International Airport', 'JFK', 2),
('George Bush Intercontinental Airport', 'IAH', 3),
('Miami International Airport', 'MIA', 4),
('O\'Hare International Airport', 'ORD', 5),
('McCarran International Airport', 'LAS', 6),
('Denver International Airport', 'DEN', 7),
('Seattle-Tacoma International Airport', 'SEA', 8),
('Logan International Airport', 'BOS', 9),
('Phoenix Sky Harbor International Airport', 'PHX', 10);

-- Sample data for `users`
INSERT INTO `airline_project`.`users` (`email`, `password`, `role`) VALUES
('admin@example.com', 'AQAAAAIAAYagAAAAECrPK/OaPJ1TtlTl0to+E6f86of8ocaDjTNDunN9fPPlIpFUd787gRpL2lusp8srkg==', 'Admin'),
('admin2@example.com', 'AQAAAAIAAYagAAAAECrPK/OaPJ1TtlTl0to+E6f86of8ocaDjTNDunN9fPPlIpFUd787gRpL2lusp8srkg==', 'Admin'),
('customer@example.com', 'AQAAAAIAAYagAAAAEJvAdN3g69LF6cuKWK/xIHyUyz1qtNoVCMgKIlSd5oTPwk+7/A+qEAcxQJ2B+FvghQ==', 'Customer'),
('customer2@example.com', 'AQAAAAIAAYagAAAAEJvAdN3g69LF6cuKWK/xIHyUyz1qtNoVCMgKIlSd5oTPwk+7/A+qEAcxQJ2B+FvghQ==', 'Customer'),
('customer3@example.com', 'AQAAAAIAAYagAAAAEJvAdN3g69LF6cuKWK/xIHyUyz1qtNoVCMgKIlSd5oTPwk+7/A+qEAcxQJ2B+FvghQ==', 'Customer'),
('customer4@example.com', 'AQAAAAIAAYagAAAAEJvAdN3g69LF6cuKWK/xIHyUyz1qtNoVCMgKIlSd5oTPwk+7/A+qEAcxQJ2B+FvghQ==', 'Customer'),
('customer5@example.com', 'AQAAAAIAAYagAAAAEJvAdN3g69LF6cuKWK/xIHyUyz1qtNoVCMgKIlSd5oTPwk+7/A+qEAcxQJ2B+FvghQ==', 'Customer');

-- Sample data for `bookings`
INSERT INTO `airline_project`.`bookings` (`confirmation_number`, `user_id`) VALUES
('ABC123', 2),
('DEF456', 2),
('GHI789', 3),
('JKL012', 3),
('MNO345', 4),
('PQR678', 4),
('STU901', 5),
('VWX234', 5),
('YZA567', 5),
('BCD890', 5);

-- Sample data for `flight_classes`
INSERT INTO `airline_project`.`flight_classes` (`name`, `price_multiplier`) VALUES
('EconomyClass', 1.00),
('BusinessClass', 1.50),
('FirstClass', 3.00);

-- Sample data for `flights`
INSERT INTO `airline_project`.`flights` 
(`flight_code`, `departure_port`, `arrival_port`, `departure_time`, `travel_time`, `completion_time`, `price`, `kilometers`, `economy_class_seats_available`, `business_class_seats_available`, `first_class_seats_available`, `flights_airline_id`, `flights_airplane_id`, `idempotency_key`, `created_by`, `updated_by`) 
VALUES
('DL096', 1, 2, '2024-12-24 02:30:00', 360, '2024-12-24 10:30:00', 199.99, 450, 150, 20, 5, 1, 5, 'TEST_DL096', "test@gmail.com", "test@gmail.com"),
('DL097', 1, 2, '2024-12-24 07:30:00', 360, '2024-12-24 15:30:00', 199.99, 450, 150, 20, 5, 1, 10, 'TEST_DL097', "test@gmail.com", "test@gmail.com"),
('DL098', 1, 2, '2024-12-24 08:00:00', 360, '2024-12-24 16:00:00', 199.99, 450, 150, 20, 5, 1, 8, 'TEST_DL098', "test@gmail.com", "test@gmail.com"),
('DL099', 1, 2, '2024-12-24 08:00:00', 360, '2024-12-24 16:00:00', 199.99, 450, 150, 20, 5, 1, 9, 'TEST_DL099', "test@gmail.com", "test@gmail.com"),
('DL100', 1, 2, '2024-12-24 08:00:00', 360, '2024-12-24 16:00:00', 199.99, 450, 150, 20, 5, 1, 1, 'TEST_DL100', "test@gmail.com", "test@gmail.com"),
('DL101', 1, 2, '2024-12-24 09:30:00', 360, '2024-12-24 17:30:00', 199.99, 450, 150, 20, 5, 1, 2, 'TEST_DL101', "test@gmail.com", "test@gmail.com"),
('DL102', 1, 2, '2024-12-24 11:30:00', 360, '2024-12-24 19:30:00', 199.99, 450, 150, 20, 5, 1, 3, 'TEST_DL102', "test@gmail.com", "test@gmail.com"),
('DL103', 1, 2, '2024-12-24 13:30:00', 360, '2024-12-24 21:30:00', 199.99, 450, 150, 20, 5, 1, 4, 'TEST_DL103', "test@gmail.com", "test@gmail.com"),
('DL104', 1, 2, '2024-12-24 13:45:00', 360, '2024-12-24 21:45:00', 199.99, 450, 150, 20, 5, 1, 5, 'TEST_DL104', "test@gmail.com", "test@gmail.com"),
('DL105', 1, 2, '2024-12-24 14:30:00', 360, '2024-12-24 22:30:00', 199.99, 450, 150, 20, 5, 1, 6, 'TEST_DL105', "test@gmail.com", "test@gmail.com"),
('DL106', 1, 2, '2024-12-24 14:45:00', 360, '2024-12-24 22:45:00', 199.99, 450, 150, 20, 5, 1, 7, 'TEST_DL106', "test@gmail.com", "test@gmail.com"),
('DL107', 2, 1, '2025-01-02 08:00:00', 360, '2025-01-02 16:00:00', 199.99, 450, 150, 20, 5, 1, 1, 'TEST_DL107', "test@gmail.com", "test@gmail.com"),
('DL108', 2, 1, '2025-01-02 09:30:00', 360, '2025-01-02 17:30:00', 199.99, 450, 150, 20, 5, 1, 2, 'TEST_DL108', "test@gmail.com", "test@gmail.com"),
('DL109', 2, 1, '2025-01-02 11:30:00', 360, '2025-01-02 19:30:00', 199.99, 450, 150, 20, 5, 1, 3, 'TEST_DL109', "test@gmail.com", "test@gmail.com"),
('DL110', 2, 1, '2025-01-02 13:30:00', 360, '2025-01-02 21:30:00', 199.99, 450, 150, 20, 5, 1, 4, 'TEST_DL110', "test@gmail.com", "test@gmail.com"),
('DL111', 2, 1, '2025-01-02 13:45:00', 360, '2025-01-02 21:45:00', 199.99, 450, 150, 20, 5, 1, 5, 'TEST_DL111', "test@gmail.com", "test@gmail.com"),
('DL112', 2, 1, '2025-01-02 14:30:00', 360, '2025-01-02 22:30:00', 199.99, 450, 150, 20, 5, 1, 6, 'TEST_DL112', "test@gmail.com", "test@gmail.com"),
('DL113', 2, 1, '2025-01-02 14:45:00', 360, '2025-01-02 22:45:00', 199.99, 450, 150, 20, 5, 1, 7, 'TEST_DL113', "test@gmail.com", "test@gmail.com");

-- Sample data for `passengers`
INSERT INTO `airline_project`.`passengers` (`first_name`, `last_name`, `email`) VALUES
('Jane', 'Smith', 'jane.smith@example.com'),
('Michael', 'Jones', 'michael.jones@example.com'),
('Emily', 'Johnson', 'emily.johnson@example.com'),
('William', 'Brown', 'william.brown@example.com'),
('Olivia', 'Davis', 'olivia.davis@example.com'),
('Liam', 'Wilson', 'liam.wilson@example.com'),
('Sophia', 'Moore', 'sophia.moore@example.com'),
('James', 'Taylor', 'james.taylor@example.com'),
('Isabella', 'Miller', 'isabella.miller@example.com');

-- Sample data for `tickets`
INSERT INTO `airline_project`.`tickets` (`price`, `ticket_number`, `passenger_id`, `flight_id`, `tickets_booking_id`, `tickets_class_id`) VALUES
(300.00, 'TCK1001', 1, 1, 1, 1),
(500.00, 'TCK1002', 2, 2, 2, 1),
(250.00, 'TCK1003', 3, 3, 3, 1),
(450.00, 'TCK1004', 4, 4, 4, 1),
(350.00, 'TCK1005', 5, 5, 5, 2),
(600.00, 'TCK1006', 6, 6, 6, 2),
(200.00, 'TCK1007', 7, 7, 7, 2),
(550.00, 'TCK1008', 8, 8, 8, 3),
(400.00, 'TCK1009', 9, 9, 9, 3),
(700.00, 'TCK1010', 9, 8, 9, 2);
