using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightBookingApi.Models;

public class Price
{
    [Required]
    [ForeignKey("flight_id")]
    public string flight_id;

    [Key]
    public string price_id { get; set; }
    public string currency { get; set; }
    public decimal adult { get; set; }
    public decimal child { get; set; }
}