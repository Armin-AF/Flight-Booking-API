using FlightBookingApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightBookingApi.Controllers;


[ApiController]
[Route("[controller]")]
public class BookingsController : ControllerBase
{
    readonly List<Booking> _bookings = new();
    
    public BookingsController ()
    {
        _bookings.Add(new Booking
        {
            booking_id = "1",
            flight_id = "1",
            seats = 1,
            customer_id = "1"
        });
        _bookings.Add(new Booking
        {
            booking_id = "2",
            flight_id = "2",
            seats = 2,
            customer_id = "2"
        });
        _bookings.Add(new Booking
        {
            booking_id = "3",
            flight_id = "3",
            seats = 3,
            customer_id = "3"
        });
        _bookings.Add(new Booking
        {
            booking_id = "4",
            flight_id = "4",
            seats = 4,
            customer_id = "4"
        });
    }
    
    [HttpGet]
    public IActionResult GetBookings()
    {
        return Ok(_bookings);
    }
    
    [HttpGet("{id}")]
    public IActionResult GetBooking(string id)
    {
        var booking = _bookings.FirstOrDefault(booking => booking.booking_id == id);
        if (booking == null) return NotFound();
        return Ok(booking);
    }
    
    [HttpPost]
    public IActionResult PostBooking(Booking booking)
    {
        _bookings.Add(booking);
        return Ok(booking);
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteBooking(string id)
    {
        var booking = _bookings.FirstOrDefault(booking => booking.booking_id == id);
        if (booking == null) return NotFound();
        _bookings.Remove(booking);
        return Ok();
    }
    
    [HttpPut("{id}")]
    public IActionResult PutBooking(string id, Booking booking)
    {
        var bookingToUpdate = _bookings.FirstOrDefault(booking => booking.booking_id == id);
        if (bookingToUpdate == null) return NotFound();
        bookingToUpdate = booking;
        return Ok(bookingToUpdate);
    }
    
    [HttpPatch("{id}")]
    public IActionResult PatchBooking(string id, Booking booking)
    {
        var bookingToUpdate = _bookings.FirstOrDefault(booking => booking.booking_id == id);
        if (bookingToUpdate == null) return NotFound();
        bookingToUpdate = booking;
        return Ok(bookingToUpdate);
    }
}