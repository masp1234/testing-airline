-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema airline_project
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `airline_project` DEFAULT CHARACTER SET utf8mb3;
USE `airline_project`;

-- -----------------------------------------------------
-- Table `airline_project`.`airlines`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`airlines` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE = InnoDB DEFAULT CHARACTER SET = utf8mb3;

-- -----------------------------------------------------
-- Table `airline_project`.`airplanes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`airplanes` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(80) NOT NULL,
  `airplanes_airline_id` INT NOT NULL,
  `economy_class_seats` INT NOT NULL,
  `business_class_seats` INT NOT NULL,
  `first_class_seats` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `airline_id_idx` (`airplanes_airline_id` ASC) VISIBLE,
  CONSTRAINT `airplanes_airline_id`
    FOREIGN KEY (`airplanes_airline_id`)
    REFERENCES `airline_project`.`airlines` (`id`)
) ENGINE = InnoDB DEFAULT CHARACTER SET = utf8mb3;

-- -----------------------------------------------------
-- Table `airline_project`.`states`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`states` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `code` VARCHAR(2) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE = InnoDB DEFAULT CHARACTER SET = utf8mb3;

-- -----------------------------------------------------
-- Table `airline_project`.`cities`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`cities` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL,
  `state_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `state_id_idx` (`state_id` ASC) VISIBLE,
  CONSTRAINT `state_id`
    FOREIGN KEY (`state_id`)
    REFERENCES `airline_project`.`states` (`id`)
) ENGINE = InnoDB DEFAULT CHARACTER SET = utf8mb3;

-- -----------------------------------------------------
-- Table `airline_project`.`airports`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`airports` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(120) NOT NULL,
  `code` VARCHAR(20) NOT NULL,
  `city_id` INT NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  INDEX `city_id_idx` (`city_id` ASC) VISIBLE,
  CONSTRAINT `city_id`
    FOREIGN KEY (`city_id`)
    REFERENCES `airline_project`.`cities` (`id`)
) ENGINE = InnoDB DEFAULT CHARACTER SET = utf8mb3;

-- -----------------------------------------------------
-- Table `airline_project`.`users`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`users` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `email` VARCHAR(80) NOT NULL,
  `password` VARCHAR(100) NOT NULL,
  `role` VARCHAR(8) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `email_UNIQUE` (`email` ASC) VISIBLE
) ENGINE = InnoDB DEFAULT CHARACTER SET = utf8mb3;

-- -----------------------------------------------------
-- Table `airline_project`.`bookings`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`bookings` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `confirmation_number` VARCHAR(45) NOT NULL,
  `user_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `user_id_idx` (`user_id` ASC) VISIBLE,
  CONSTRAINT `user_id`
    FOREIGN KEY (`user_id`)
    REFERENCES `airline_project`.`users` (`id`)
) ENGINE = InnoDB DEFAULT CHARACTER SET = utf8mb3;

-- -----------------------------------------------------
-- Table `airline_project`.`flight_classes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`flight_classes` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NOT NULL,
  `price_multiplier` DECIMAL(3, 2),
  PRIMARY KEY (`id`)
) ENGINE = InnoDB DEFAULT CHARACTER SET = utf8mb3;

-- -----------------------------------------------------
-- Table `airline_project`.`flights`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`flights` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `flight_code` VARCHAR(45) NOT NULL,
  `departure_port` INT NOT NULL,
  `arrival_port` INT NOT NULL,
  `departure_time` DATETIME NOT NULL,
  `completion_time` DATETIME NOT NULL,
  `travel_time` INT NOT NULL,
  `price` DECIMAL(10, 2) NOT NULL,
  `kilometers` INT NULL DEFAULT NULL,
  `economy_class_seats_available` INT NOT NULL,
  `business_class_seats_available` INT NOT NULL,
  `first_class_seats_available` INT NOT NULL,
  `flights_airline_id` INT NOT NULL,
  `flights_airplane_id` INT NOT NULL,
  `idempotency_key` VARCHAR(60) NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `departure_port_idx` (`departure_port` ASC) VISIBLE,
  INDEX `arrival_port_idx` (`arrival_port` ASC) VISIBLE,
  INDEX `flights_airline_id_idx` (`flights_airline_id` ASC) VISIBLE,
  INDEX `flights_airplane_id` (`flights_airplane_id` ASC) VISIBLE,
  CONSTRAINT `arrival_port`
    FOREIGN KEY (`arrival_port`)
    REFERENCES `airline_project`.`airports` (`id`),
  CONSTRAINT `departure_port`
    FOREIGN KEY (`departure_port`)
    REFERENCES `airline_project`.`airports` (`id`),
  CONSTRAINT `flights_airline_id`
    FOREIGN KEY (`flights_airline_id`)
    REFERENCES `airline_project`.`airlines` (`id`),
  CONSTRAINT `flights_airplane_id`
    FOREIGN KEY (`flights_airplane_id`)
    REFERENCES `airline_project`.`airplanes` (`id`)
) ENGINE = InnoDB DEFAULT CHARACTER SET = utf8mb3;

-- -----------------------------------------------------
-- Table `airline_project`.`passengers`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`passengers` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `first_name` VARCHAR(80) NOT NULL,
  `last_name` VARCHAR(120) NOT NULL,
  `email` VARCHAR(320) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE = InnoDB DEFAULT CHARACTER SET = utf8mb3;

-- -----------------------------------------------------
-- Table `airline_project`.`tickets`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`tickets` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `price` DECIMAL(5, 2) NOT NULL,
  `ticket_number` VARCHAR(45) NOT NULL,
  `passenger_id` INT NOT NULL,
  `flight_id` INT NOT NULL,
  `tickets_booking_id` INT NOT NULL,
  `tickets_class_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `passenger_id_idx` (`passenger_id` ASC) VISIBLE,
  INDEX `flight_id_idx` (`flight_id` ASC) VISIBLE,
  INDEX `tickets_booking_id_idx` (`tickets_booking_id` ASC) VISIBLE,
  INDEX `tickets_class_id_idx` (`tickets_class_id` ASC) VISIBLE,
  CONSTRAINT `flight_id`
    FOREIGN KEY (`flight_id`)
    REFERENCES `airline_project`.`flights` (`id`),
  CONSTRAINT `passenger_id`
    FOREIGN KEY (`passenger_id`)
    REFERENCES `airline_project`.`passengers` (`id`),
  CONSTRAINT `tickets_booking_id`
    FOREIGN KEY (`tickets_booking_id`)
    REFERENCES `airline_project`.`bookings` (`id`),
  CONSTRAINT `tickets_class_id`
    FOREIGN KEY (`tickets_class_id`)
    REFERENCES `airline_project`.`flight_classes` (`id`)
) ENGINE = InnoDB DEFAULT CHARACTER SET = utf8mb3;


-- Create stored procedures

-- CheckAndInsertFlight stored procedure
DELIMITER $$
CREATE DEFINER=`root`@`%` PROCEDURE `CheckAndInsertFlight`(
    IN airplaneId INT,
    IN departureTime DATETIME,
    IN completionTime DATETIME,
    IN flightCode VARCHAR(45),
    IN departurePort INT,
    IN arrivalPort INT,
    IN travelTime INT,
    IN price DECIMAL(10, 2),
    IN kilometers INT,
    IN economySeats INT,
    IN businessSeats INT,
    IN firstClassSeats INT,
    IN airlineId INT,
    IN idempotencyKey VARCHAR(60),
    OUT newFlightId INT
)
BEGIN
    -- Start transaction
    DECLARE exit HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Transaction failed. Overlap detected or other error occurred.';
    END;

    START TRANSACTION;

    -- Check for overlapping flights
    IF EXISTS (
        SELECT 1
        FROM flights
        WHERE flights_airplane_id = airplaneId
          AND departure_time < completionTime
          AND completion_time > departureTime
    ) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Overlap detected with existing flight schedule.';
    ELSE
        -- Insert the new flight
        INSERT INTO flights 
        (
            flight_code,
            departure_port,
            arrival_port,
            departure_time,
            travel_time,
            price,
            kilometers,
            economy_class_seats_available,
            business_class_seats_available,
            first_class_seats_available,
            flights_airline_id,
            flights_airplane_id,
            completion_time,
            idempotency_key
        )
        VALUES 
        (
            flightCode,
            departurePort,
            arrivalPort,
            departureTime,
            travelTime,
            price,
            kilometers,
            economySeats,
            businessSeats,
            firstClassSeats,
            airlineId,
            airplaneId,
            completionTime,
            idempotencyKey
        );

        -- Get the ID of the newly inserted flight
        SET newFlightId = LAST_INSERT_ID();
    END IF;

    -- Commit the transaction
    COMMIT;
END$$
DELIMITER ;
-- CheckAndInsertFlight stored procedure end


-- -----------------------------------------------------
-- Application User Role Creation
-- -----------------------------------------------------
CREATE ROLE app_user;

GRANT SELECT, CREATE ON TABLE users TO app_user;

GRANT SELECT ON TABLE airplanes TO app_user;

GRANT SELECT ON TABLE airports TO app_user;

GRANT SELECT ON TABLE airlines TO app_user;

GRANT SELECT, CREATE ON TABLE flights TO app_user;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;


