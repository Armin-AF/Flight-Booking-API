namespace FlightBookingApi.Models;

public class Flight
{
    public IEnumerable<Booking> bookings;
    public string flight_id { get; set; }
    public DateTime departureAt { get; set; }
    public DateTime arrivalAt { get; set; }
    public int availableSeats { get; set; }
    public Price prices { get; set; }
    public TimeSpan layoverDuration{ get; set; } = TimeSpan.Zero;
}