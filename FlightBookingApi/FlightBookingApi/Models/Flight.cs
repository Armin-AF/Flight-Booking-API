namespace FlightBookingApi.Models;

public class Flight
{
    public int Id { get; set; }
    public string DepartureDestination { get; set; }
    public string ArrivalDestination { get; set; }
    public DateTime DepartureDate { get; set; }
    public DateTime ArrivalDate { get; set; }
    public int AvailableSeats { get; set; }
    public Price Price { get; set; }
}