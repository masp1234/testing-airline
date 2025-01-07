namespace backend.Dtos
{
    public class AirplaneBookedTimeSlot
    {

        public long FlightId { get; set; }

        public DateTime TimeSlotStart { get; set; }

        public DateTime TimeSlotEnd { get; set; }
    }
}
