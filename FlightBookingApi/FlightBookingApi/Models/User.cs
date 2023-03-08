namespace FlightBookingApi.Models;

public class User
{
    public string user_id { get; set; }
    public string name { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public string role { get; set; }
}