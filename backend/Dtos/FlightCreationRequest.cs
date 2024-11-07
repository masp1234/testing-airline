namespace backend.Dtos
{
    public class FlightCreationRequest
    {
        public int AirlineId { get; set; }

        public int AirplaneId { get; set; }

        public int DepartureAirportId { get; set; }

        public int ArrivalAirportId { get; set; }

        public DateTime DepartureDateTime { get; set; }

        public double Price { get; set; }

        public string IdempotencyKey { get; set; }
    }
}
