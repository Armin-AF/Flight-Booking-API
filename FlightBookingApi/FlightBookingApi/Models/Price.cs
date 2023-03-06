namespace FlightBookingApi.Models;

public class Price
{
    public string Currency { get; set; }
    public double AdultPrice { get; set; }
    public double ChildPrice { get; set; }
}