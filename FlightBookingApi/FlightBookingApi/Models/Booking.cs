namespace FlightBookingApi.Models;

public class Booking
{
    public string booking_id { get; set; }
    public string flight_id { get; set; }
    public int seats { get; set; }
    public string customer_id { get; set; }
    
}