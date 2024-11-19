namespace backend.Dtos
{
    public class AirplaneBookedTimeSlot
    {

        public int FlightId { get; set; }

        public DateTime TimeSlotStart { get; set; }

        public DateTime TimeSlotEnd { get; set; }
    }
}
