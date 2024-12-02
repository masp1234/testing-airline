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
(`flight_code`, `departure_port`, `arrival_port`, `departure_time`, `travel_time`, `completion_time`, `price`, `kilometers`, `economy_class_seats_available`, `business_class_seats_available`, `first_class_seats_available`, `flights_airline_id`, `flights_airplane_id`, `idempotency_key`) 
VALUES
('DL100', 1, 2, '2024-12-01 08:00:00', 360, '2024-12-01 16:00:00', 199.99, 450, 150, 20, 5, 1, 1, 'TEST_DL100'),
('UA200', 2, 3, '2024-12-01 10:00:00', 240, '2024-12-01 16:00:00', 149.99, 300, 120, 15, 3, 2, 2, 'TEST_UA200'),
('AA300', 3, 4, '2024-12-02 12:00:00', 180, '2024-12-02 17:00:00', 129.99, 200, 100, 10, 2, 3, 3, 'TEST_AA300'),
('SW400', 4, 5, '2024-12-03 14:00:00', 300, '2024-12-03 23:00:00', 179.99, 500, 180, 25, 4, 3, 3, 'TEST_SW400'),
('JB500', 5, 6, '2024-12-04 16:00:00', 420, '2024-12-05 04:00:00', 209.99, 600, 200, 30, 6, 4, 4, 'TEST_JB500'),
('AL600', 6, 7, '2024-12-05 18:00:00', 180, '2024-12-06 01:00:00', 159.99, 250, 140, 18, 3, 6, 5, 'TEST_AL600'),
('SP700', 7, 8, '2024-12-06 20:00:00', 360, '2024-12-07 08:00:00', 189.99, 550, 160, 20, 5, 7, 6, 'TEST_SP700'),
('FR800', 8, 9, '2024-12-07 22:00:00', 240, '2024-12-08 08:00:00', 169.99, 400, 110, 12, 2, 8, 7, 'TEST_FR800'),
('HA900', 9, 10, '2024-12-08 06:00:00', 540, '2024-12-08 19:00:00', 299.99, 800, 220, 35, 7, 9, 8, 'TEST_HA900'),
('AL1000', 10, 1, '2024-12-09 08:00:00', 300, '2024-12-09 17:00:00', 179.99, 450, 130, 10, 3, 10, 10, 'TEST_AL1000');



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

-- Sample data for `invoices`
INSERT INTO `airline_project`.`invoices` (`amount_due`, `due_date`, `date_paid`, `is_paid`, `invoice_booking_id`) VALUES
(300.00, '2024-11-01', '2024-11-15', 1, 1),
(500.00, '2024-11-05', '2024-11-20', 1, 2),
(250.00, '2024-11-08', NULL, 0, 3),
(450.00, '2024-11-12', '2024-11-22', 1, 4),
(350.00, '2024-11-18', '2024-11-25', 1, 5),
(600.00, '2024-11-25', NULL, 0, 6),
(200.00, '2024-11-30', '2024-12-05', 1, 7),
(550.00, '2024-12-03', '2024-12-10', 1, 8),
(400.00, '2024-12-08', NULL, 0, 9),
(700.00, '2024-12-12', '2024-12-20', 1, 10);
