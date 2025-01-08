namespace backend.Dtos
{
    public class UpdateFlightRequest
    {
        [NotDefaultDate]
        public DateTime DepartureDateTime { get; set; }
    }
}
