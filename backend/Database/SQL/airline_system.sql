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
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE = InnoDB DEFAULT CHARACTER SET = utf8mb3;

-- -----------------------------------------------------
-- Table `airline_project`.`airplanes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`airplanes` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(80) NOT NULL,
  `airplanes_airline_id` BIGINT NOT NULL,
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
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `code` VARCHAR(2) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE = InnoDB DEFAULT CHARACTER SET = utf8mb3;

-- -----------------------------------------------------
-- Table `airline_project`.`cities`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`cities` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL,
  `state_id` BIGINT NOT NULL,
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
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(120) NOT NULL,
  `code` VARCHAR(20) NOT NULL,
  `city_id` BIGINT NULL DEFAULT NULL,
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
  `id` BIGINT NOT NULL AUTO_INCREMENT,
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
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `confirmation_number` VARCHAR(45) NOT NULL,
  `user_id` BIGINT NOT NULL,
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
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NOT NULL,
  `price_multiplier` DECIMAL(3, 2),
  PRIMARY KEY (`id`)
) ENGINE = InnoDB DEFAULT CHARACTER SET = utf8mb3;

-- -----------------------------------------------------
-- Table `airline_project`.`flights`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`flights` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `flight_code` VARCHAR(45) NOT NULL,
  `departure_port` BIGINT NOT NULL,
  `arrival_port` BIGINT NOT NULL,
  `departure_time` DATETIME NOT NULL,
  `completion_time` DATETIME NOT NULL,
  `travel_time` INT NOT NULL,
  `price` DECIMAL(10, 2) NOT NULL,
  `kilometers` INT NULL DEFAULT NULL,
  `economy_class_seats_available` INT NOT NULL,
  `business_class_seats_available` INT NOT NULL,
  `first_class_seats_available` INT NOT NULL,
  `flights_airline_id` BIGINT NOT NULL,
  `flights_airplane_id` BIGINT NOT NULL,
  `idempotency_key` VARCHAR(60) NOT NULL,
  `created_by` VARCHAR(100) NOT NULL,
  `updated_by` VARCHAR(100) NOT NULL,
  `version` INT NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`),
  INDEX `departure_port_idx` (`departure_port` ASC) VISIBLE,
  INDEX `arrival_port_idx` (`arrival_port` ASC) VISIBLE,
  INDEX `flights_airline_id_idx` (`flights_airline_id` ASC) VISIBLE,
  INDEX `flights_airplane_id` (`flights_airplane_id` ASC) VISIBLE,
  INDEX `departure_time_idx` (`departure_time` ASC) VISIBLE,
  INDEX `departure_completion_idx` (`departure_time` ASC, `completion_time` ASC) VISIBLE,
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
-- Table `airline_project`.`flights_audit`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`flights_audit` (
    `audit_id` BIGINT NOT NULL AUTO_INCREMENT,
    `flight_id` BIGINT NOT NULL,
    `column_name` VARCHAR(255) NOT NULL,
    `old_value` TEXT NULL,
    `new_value` TEXT NULL,
    `operation` ENUM('INSERT', 'UPDATE', 'DELETE') NOT NULL,
    `done_by` VARCHAR(100) NULL,
    `operation_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`audit_id`),
    INDEX `flight_id_idx` (`flight_id` ASC)
) ENGINE = InnoDB DEFAULT CHARACTER SET = utf8mb3;

-- -----------------------------------------------------
-- Table `airline_project`.`passengers`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`passengers` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `first_name` VARCHAR(80) NOT NULL,
  `last_name` VARCHAR(120) NOT NULL,
  `email` VARCHAR(320) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE = InnoDB DEFAULT CHARACTER SET = utf8mb3;

-- -----------------------------------------------------
-- Table `airline_project`.`tickets`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`tickets` (
  `id` BIGINT NOT NULL AUTO_INCREMENT,
  `price` DECIMAL(5, 2) NOT NULL,
  `ticket_number` VARCHAR(45) NOT NULL,
  `passenger_id` BIGINT NOT NULL,
  `flight_id` BIGINT NOT NULL,
  `tickets_booking_id` BIGINT NOT NULL,
  `tickets_class_id` BIGINT NOT NULL,
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
    IN airplaneId BIGINT,
    IN departureTime DATETIME,
    IN completionTime DATETIME,
    IN flightCode VARCHAR(45),
    IN departurePort BIGINT,
    IN arrivalPort BIGINT,
    IN travelTime INT,
    IN price DECIMAL(10, 2),
    IN kilometers INT,
    IN economySeats INT,
    IN businessSeats INT,
    IN firstClassSeats INT,
    IN airlineId BIGINT,
    IN idempotencyKey VARCHAR(60),
    IN createdBy VARCHAR(100),
    OUT newFlightId BIGINT
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
            idempotency_key,
            created_by,
            updated_by
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
            idempotencyKey,
            createdBy,
            createdBy
        );

        -- Get the ID of the newly inserted flight
        SET newFlightId = LAST_INSERT_ID();
    END IF;

    -- Commit the transaction
    COMMIT;
END$$
DELIMITER ;
-- CheckAndInsertFlight stored procedure end

-- Triggers for flight inserts, updates and deletes

-- Insert trigger
DELIMITER $$
CREATE TRIGGER flights_after_insert
AFTER INSERT ON `airline_project`.`flights`
FOR EACH ROW
BEGIN
    INSERT INTO `airline_project`.`flights_audit` (`flight_id`, `column_name`, `new_value`, `operation`, `done_by`)
    VALUES 
        (NEW.id, 'flight_code', NEW.flight_code, 'INSERT', NEW.created_by),
        (NEW.id, 'departure_port', NEW.departure_port, 'INSERT', NEW.created_by),
        (NEW.id, 'arrival_port', NEW.arrival_port, 'INSERT', NEW.created_by),
        (NEW.id, 'departure_time', NEW.departure_time, 'INSERT', NEW.created_by),
        (NEW.id, 'completion_time', NEW.completion_time, 'INSERT', NEW.created_by),
        (NEW.id, 'travel_time', NEW.travel_time, 'INSERT', NEW.created_by),
        (NEW.id, 'price', NEW.price, 'INSERT', NEW.created_by),
        (NEW.id, 'kilometers', NEW.kilometers, 'INSERT', NEW.created_by),
        (NEW.id, 'economy_class_seats_available', NEW.economy_class_seats_available, 'INSERT', NEW.created_by),
        (NEW.id, 'business_class_seats_available', NEW.business_class_seats_available, 'INSERT', NEW.created_by),
        (NEW.id, 'first_class_seats_available', NEW.first_class_seats_available, 'INSERT', NEW.created_by),
        (NEW.id, 'flights_airline_id', NEW.flights_airline_id, 'INSERT', NEW.created_by),
        (NEW.id, 'flights_airplane_id', NEW.flights_airplane_id, 'INSERT', NEW.created_by);
END$$
DELIMITER ;

-- Update trigger
DELIMITER $$
CREATE TRIGGER flights_after_update
AFTER UPDATE ON `airline_project`.`flights`
FOR EACH ROW
BEGIN
    IF OLD.departure_time != NEW.departure_time THEN
        INSERT INTO `airline_project`.`flights_audit` 
            (`flight_id`, `column_name`, `old_value`, `new_value`, `operation`, `done_by`)
        VALUES 
            (NEW.id, 'departure_time', OLD.departure_time, NEW.departure_time, 'UPDATE', NEW.updated_by);
    END IF;

    IF OLD.completion_time != NEW.completion_time THEN
        INSERT INTO `airline_project`.`flights_audit` 
            (`flight_id`, `column_name`, `old_value`, `new_value`, `operation`, `done_by`)
        VALUES 
            (NEW.id, 'completion_time', OLD.completion_time, NEW.completion_time, 'UPDATE', NEW.updated_by);
    END IF;
END$$
DELIMITER ;

-- Delete trigger
DELIMITER $$
CREATE TRIGGER flights_after_delete
AFTER DELETE ON `airline_project`.`flights`
FOR EACH ROW
BEGIN
    INSERT INTO `airline_project`.`flights_audit` (`flight_id`, `column_name`, `old_value`, `operation`, `done_by`)
    VALUES 
        (OLD.id, 'flight_code', OLD.flight_code, 'DELETE', @deleted_by_email),
        (OLD.id, 'departure_port', OLD.departure_port, 'DELETE', @deleted_by_email),
        (OLD.id, 'arrival_port', OLD.arrival_port, 'DELETE', @deleted_by_email),
        (OLD.id, 'departure_time', OLD.departure_time, 'DELETE', @deleted_by_email),
        (OLD.id, 'completion_time', OLD.completion_time, 'DELETE', @deleted_by_email),
        (OLD.id, 'travel_time', OLD.travel_time, 'DELETE', @deleted_by_email),
        (OLD.id, 'price', OLD.price, 'DELETE', @deleted_by_email),
        (OLD.id, 'kilometers', OLD.kilometers, 'DELETE', @deleted_by_email),
        (OLD.id, 'economy_class_seats_available', OLD.economy_class_seats_available, 'DELETE', @deleted_by_email),
        (OLD.id, 'business_class_seats_available', OLD.business_class_seats_available, 'DELETE', @deleted_by_email),
        (OLD.id, 'first_class_seats_available', OLD.first_class_seats_available, 'DELETE', @deleted_by_email),
        (OLD.id, 'flights_airline_id', OLD.flights_airline_id, 'DELETE', @deleted_by_email),
        (OLD.id, 'flights_airplane_id', OLD.flights_airplane_id, 'DELETE', @deleted_by_email);
END$$
DELIMITER ;



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


