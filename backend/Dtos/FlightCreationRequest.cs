namespace backend.Dtos
{
    public class FlightCreationRequest
    {
        public long AirlineId { get; set; }

        public long AirplaneId { get; set; }

        public long DepartureAirportId { get; set; }

        public long ArrivalAirportId { get; set; }

        public DateTime DepartureDateTime { get; set; }

        public double Price { get; set; }

        public string IdempotencyKey { get; set; }

        public string? CreatedBy { get; set; }
    }
}
