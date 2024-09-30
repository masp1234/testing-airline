-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema airline_project
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema airline_project
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `airline_project` DEFAULT CHARACTER SET utf8mb3 ;
USE `airline_project` ;

-- -----------------------------------------------------
-- Table `airline_project`.`airlines`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`airlines` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;


-- -----------------------------------------------------
-- Table `airline_project`.`airplanes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`airplanes` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(80) NOT NULL,
  `airplanes_airline_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `airline_id_idx` (`airplanes_airline_id` ASC) VISIBLE,
  CONSTRAINT `airplanes_airline_id`
    FOREIGN KEY (`airplanes_airline_id`)
    REFERENCES `airline_project`.`airlines` (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;


-- -----------------------------------------------------
-- Table `airline_project`.`states`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`states` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `code` VARCHAR(2) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;


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
    REFERENCES `airline_project`.`states` (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;


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
    REFERENCES `airline_project`.`cities` (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;


-- -----------------------------------------------------
-- Table `airline_project`.`users`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`users` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `email` VARCHAR(80) NOT NULL,
  `password` VARCHAR(25) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `email_UNIQUE` (`email` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;


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
    REFERENCES `airline_project`.`users` (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;


-- -----------------------------------------------------
-- Table `airline_project`.`flight_classes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`flight_classes` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;


-- -----------------------------------------------------
-- Table `airline_project`.`flights`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`flights` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `flight_code` VARCHAR(45) NOT NULL,
  `departure_port` INT NOT NULL,
  `arrival_port` INT NOT NULL,
  `departure_time` DATETIME NOT NULL,
  `travel_time` INT NOT NULL,
  `kilometers` VARCHAR(45) NULL DEFAULT NULL,
  `flights_airline_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `departure_port_idx` (`departure_port` ASC) VISIBLE,
  INDEX `arrival_port_idx` (`arrival_port` ASC) VISIBLE,
  INDEX `flights_airline_id_idx` (`flights_airline_id` ASC) VISIBLE,
  CONSTRAINT `arrival_port`
    FOREIGN KEY (`arrival_port`)
    REFERENCES `airline_project`.`airports` (`id`),
  CONSTRAINT `departure_port`
    FOREIGN KEY (`departure_port`)
    REFERENCES `airline_project`.`airports` (`id`),
  CONSTRAINT `flights_airline_id`
    FOREIGN KEY (`flights_airline_id`)
    REFERENCES `airline_project`.`airlines` (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;


-- -----------------------------------------------------
-- Table `airline_project`.`invoices`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`invoices` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `amount_due` DOUBLE NOT NULL,
  `due_date` DATE NOT NULL,
  `date_paid` DATE NULL DEFAULT NULL,
  `is_paid` TINYINT NOT NULL,
  `invoice_booking_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `invoice_booking_id_idx` (`invoice_booking_id` ASC) VISIBLE,
  CONSTRAINT `invoice_booking_id`
    FOREIGN KEY (`invoice_booking_id`)
    REFERENCES `airline_project`.`bookings` (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;


-- -----------------------------------------------------
-- Table `airline_project`.`passengers`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`passengers` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(120) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;


-- -----------------------------------------------------
-- Table `airline_project`.`seats`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`seats` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `identifier` VARCHAR(45) NOT NULL,
  `airplane_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `airplane_id_idx` (`airplane_id` ASC) VISIBLE,
  CONSTRAINT `airplane_id`
    FOREIGN KEY (`airplane_id`)
    REFERENCES `airline_project`.`airplanes` (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;


-- -----------------------------------------------------
-- Table `airline_project`.`tickets`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `airline_project`.`tickets` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `price` DOUBLE NOT NULL,
  `ticket_number` VARCHAR(45) NOT NULL,
  `passenger_id` INT NOT NULL,
  `flight_id` INT NOT NULL,
  `flight_class_id` INT NOT NULL,
  `tickets_booking_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `flight_class_id_idx` (`flight_class_id` ASC) VISIBLE,
  INDEX `passenger_id_idx` (`passenger_id` ASC) VISIBLE,
  INDEX `flight_id_idx` (`flight_id` ASC) VISIBLE,
  INDEX `¨tickets_booking_id_idx` (`tickets_booking_id` ASC) VISIBLE,
  CONSTRAINT `flight_class_id`
    FOREIGN KEY (`flight_class_id`)
    REFERENCES `airline_project`.`flight_classes` (`id`),
  CONSTRAINT `flight_id`
    FOREIGN KEY (`flight_id`)
    REFERENCES `airline_project`.`flights` (`id`),
  CONSTRAINT `passenger_id`
    FOREIGN KEY (`passenger_id`)
    REFERENCES `airline_project`.`passengers` (`id`),
  CONSTRAINT `¨tickets_booking_id`
    FOREIGN KEY (`tickets_booking_id`)
    REFERENCES `airline_project`.`bookings` (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb3;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
