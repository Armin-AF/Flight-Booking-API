using System.ComponentModel.DataAnnotations;

namespace FlightBookingApi.Models;

public class Customer
{
    [Key]
    public string customer_id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string first_name { get; set; }
    
    [Required]
    [StringLength(50)]
    public string last_name { get; set; }
    
    [Required]
    [EmailAddress]
    public string email { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime date_of_birth { get; set; }
    
    public virtual ICollection<Booking> bookings { get; set; }
}