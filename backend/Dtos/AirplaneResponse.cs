namespace backend.Dtos
{
    public class AirplaneResponse
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public long AirplanesAirlineId { get; set; }

        public int EconomyClassSeats { get; set; }

        public int BusinessClassSeats { get; set; }

        public int FirstClassSeats { get; set; }
    }
}
