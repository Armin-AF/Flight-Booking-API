namespace FlightBookingApi.Models;

public class Price
{
    public string currency { get; set; }
    public decimal adult { get; set; }
    public decimal child { get; set; }
}