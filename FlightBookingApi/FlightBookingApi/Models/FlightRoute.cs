using System.ComponentModel.DataAnnotations;

namespace FlightBookingApi.Models;

public class FlightRoute
{
    [Key]
    public string route_id { get; set; }
    public string departureDestination { get; set; }
    public string arrivalDestination { get; set; }
    public virtual ICollection<Flight> itineraries { get; set; }
}