using FlightBookingApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightBookingApi.Controllers;


[ApiController]
[Route("[controller]")]
public class BookingsController : ControllerBase
{
    readonly List<Booking> _bookings = new();
    readonly List<FlightRoute> _flightRoutes;

    public BookingsController ()
    {
        _bookings.Add(new Booking
        {
            booking_id = "1",
            flight_id = "87211f8b",
            seats = 1,
            customer_id = "1"
        });
        _bookings.Add(new Booking
        {
            booking_id = "2",
            flight_id = "87211f8b",
            seats = 2,
            customer_id = "2"
        });
        _bookings.Add(new Booking
        {
            booking_id = "3",
            flight_id = "87211f8b",
            seats = 3,
            customer_id = "3"
        });
        _bookings.Add(new Booking
        {
            booking_id = "4",
            flight_id = "87211f8b",
            seats = 4,
            customer_id = "4"
        });
        
        var json = System.IO.File.ReadAllText("DataBase/data.json");
        _flightRoutes = System.Text.Json.JsonSerializer.Deserialize<List<FlightRoute>>(json)!; 
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
        if (booking.seats < 1)
        {
            return BadRequest();
        }

        var flight = _flightRoutes.FirstOrDefault(flightRoute => flightRoute.itineraries.Any(flight => flight.flight_id == booking.flight_id));
        if (flight == null)
        {
            return NotFound();
        }

        var itinerary = flight.itineraries.FirstOrDefault(f => f.flight_id == booking.flight_id);
        if (itinerary?.availableSeats < booking.seats)
        {
            return BadRequest();
        }

        _bookings.Add(booking);

        var bookingId = _bookings.Count;
        return CreatedAtAction(nameof(GetBooking), new { id = bookingId }, booking);
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
    public IActionResult PutBooking(string id, Booking booking){

        var bookingToUpdate = _bookings.FirstOrDefault(b => b.booking_id == id);
        if (bookingToUpdate == null) return NotFound();
        if (booking.seats < 1) return BadRequest();
        
        var flight = _flightRoutes.FirstOrDefault(flightRoute =>
            flightRoute.itineraries.Any(flight => flight.flight_id == booking.flight_id));
        
        if (flight == null) return NotFound();
        
        var seatsAvailable = flight.itineraries.FirstOrDefault(f => f.flight_id == booking.flight_id)?.availableSeats;
        
        if (seatsAvailable == null || seatsAvailable < booking.seats) return BadRequest();
        
        bookingToUpdate.seats = booking.seats;
        bookingToUpdate.flight_id = booking.flight_id;
        bookingToUpdate.customer_id = booking.customer_id;
        
        return Ok(bookingToUpdate);
    }
}