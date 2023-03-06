namespace FlightBookingApi.Models;

public class FlightRoute
{
    public string route_id { get; set; }
    public string departureDestination { get; set; }
    public string arrivalDestination { get; set; }
    public List<Itinerary> itineraries { get; set; }
}