using System.Globalization;

public class TicketResponse {

    public decimal Price {get; set;}
	public string TicketNumber {get; set;}
	public string FlightCode { get; set; } = null!;
	public string DeparturePortName { get; set; } = null!;
	public string ArrivalPortName { get; set; } = null!;
	public string FlightClassName { get; set; } = null!;
	public int FlightTravelTime { get; set; }
	public DateTime FlightDepartureTime { get; set; }
	public DateTime FlightCompletionTime { get; set; }
	public string PassengerFirstName { get; set; } = null!;
	public string PassengerLastName { get; set; } = null!;
	public string PassengerEmail { get; set; } = null!;
}