using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightBookingApi.Models;

public class Booking
{
    [Key]
    public string booking_id { get; set; }
    [Required]
    [ForeignKey("flight_id")]
    public string flight_id { get; set; }
    public int seats { get; set; }
    [Required]
    [ForeignKey("customer_id")]
    public string customer_id { get; set; }
    
}