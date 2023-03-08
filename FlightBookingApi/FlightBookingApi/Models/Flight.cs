using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightBookingApi.Models;

public class Flight
{
    public FlightRoute? route;
    [Required]
    [ForeignKey("route_id")]
    public string route_id;
    public virtual ICollection<Booking> bookings { get; set; }
    [Required]
    [Key]
    public string flight_id { get; set; }
    
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime departureAt { get; set; }
    [Required]
    [DataType(DataType.Date)]
    public DateTime arrivalAt { get; set; }
    [Required]
    public int availableSeats { get; set; }
    public virtual Price prices { get; set; }
    public TimeSpan layoverDuration{ get; set; } = TimeSpan.Zero;
}