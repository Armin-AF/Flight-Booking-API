namespace FlightBookingApi.Models;

public class FlightRoute
{
    public int Id { get; set; }
    public string DepartureDestination { get; set; }
    public string ArrivalDestination { get; set; }
    public List<Flight> Flights { get; set; }
}