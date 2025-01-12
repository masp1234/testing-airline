using System.ComponentModel.DataAnnotations;

namespace backend.Dtos
{
    public class FlightCreationRequest
    {

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "airlineId must be greater than 0.")]
        public int AirlineId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "airplaneId must be greater than 0.")]
        public int AirplaneId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "departureAirportId must be greater than 0.")]
        public int DepartureAirportId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "arrivalAirportId must be greater than 0.")]
        public int ArrivalAirportId { get; set; }
      
        [Required]
        // Custom validation attribute that checks that a date is not the default value
        [NotDefaultDate]
        public DateTime DepartureDateTime { get; set; }

        [Required]
        public string IdempotencyKey { get; set; }

        public string? CreatedBy { get; set; }
    }
}
