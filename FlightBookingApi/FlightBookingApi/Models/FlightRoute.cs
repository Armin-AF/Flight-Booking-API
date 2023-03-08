using System.ComponentModel.DataAnnotations;

namespace FlightBookingApi.Models;

public class FlightRoute
{
    [Key]
    public string route_id { get; set; }
    
    [Required]
    public string departureDestination { get; set; }
    
    [Required]
    public string arrivalDestination { get; set; }
    public virtual ICollection<Flight> itineraries { get; set; }
}