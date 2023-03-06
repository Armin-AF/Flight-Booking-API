namespace FlightBookingApi.Models;

public class Itinerary
{
    public int FlightId { get; set; }
    public int DurationInMinutes { get; set; }
    public int LayoverInMinutes { get; set; }
}