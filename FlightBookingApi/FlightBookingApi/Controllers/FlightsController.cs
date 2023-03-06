using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using FlightBookingApi.Models;

namespace FlightBookingApi.Controllers;

[ApiController]
[Route("[controller]")]
public class FlightsController : ControllerBase
{
    private readonly List<FlightRoute>? _flightRoutes;
    // Add logger
    private readonly ILogger<FlightsController> _logger;

    public FlightsController(ILogger<FlightsController> logger)
    {
        _logger = logger;
        // Load the flight routes from the JSON file
        var json = System.IO.File.ReadAllText("DataBase/data.json");
        _flightRoutes = System.Text.Json.JsonSerializer.Deserialize<List<FlightRoute>>(json);
    }
    
    [HttpGet]
    public IActionResult GetFlights()
    {
        return Ok(_flightRoutes);
    }
    
    [HttpGet("{id}")]
    public IActionResult GetFlightRoute(int id)
    {
        var flight = _flightRoutes!.FirstOrDefault(flightRoute => flightRoute.route_id == id.ToString());
        if (flight == null)
        {
            return NotFound();
        }
        return Ok(flight);
    }
    
    
    //Choosing flights depending on given destinations with  Have flights with layovers. You should then connect existing flights with each other, if a direct flight doesn't exist. For example; someone searches for Stockholm to Amsterdam. You don’t have any direct flight in your db for this, but you do have flights for Stockholm -> Oslo and Oslo -> Amsterdam. Then combine these two and present them as one flight, showing time for each flight and wait time between flights.  

    [HttpGet("from/{from}/to/{to}")]
    public IActionResult GetFlightRoutes(string from, string to)
    {
        var flights = _flightRoutes!.Where(flightRoute => flightRoute.departureDestination == from && flightRoute.arrivalDestination == to);
        if (!flights.Any())
        {
            var flightsWithLayovers = _flightRoutes.Where(flightRoute => flightRoute.departureDestination == from)
                .SelectMany(flightRoute => flightRoute.itineraries)
                .SelectMany(flight => _flightRoutes.Where(flightRoute => flightRoute.departureDestination == flight.arrivalAt.ToString("yyyy-MM-ddTHH:mm:ss") && flightRoute.arrivalDestination == to)
                    .SelectMany(flightRoute => flightRoute.itineraries)
                    .Select(flightRoute => new Flight
                    {
                        flight_id = flight.flight_id + flightRoute.flight_id,
                        departureAt = flight.departureAt,
                        arrivalAt = flightRoute.arrivalAt,
                        availableSeats = flight.availableSeats,
                        prices = new Price
                        {
                            currency = flight.prices.currency,
                            adult = flight.prices.adult + flightRoute.prices.adult,
                            child = flight.prices.child + flightRoute.prices.child
                        }
                    }));
            if (!flightsWithLayovers.Any())
            {
                return NotFound();
            }
            return Ok(flightsWithLayovers);
        }
        return Ok(flights.SelectMany(flightRoute => flightRoute.itineraries));
    }
    
    //Set a price-range in your search
    [HttpGet("from/{from}/to/{to}/minPrice/{minPrice}/maxPrice/{maxPrice}")]
    public IActionResult GetFlightRoutes(string from, string to, decimal minPrice, decimal maxPrice)
    {
        var flights = _flightRoutes!.Where(flightRoute => flightRoute.departureDestination == from && flightRoute.arrivalDestination == to);
        if (!flights.Any())
        {
            return NotFound();
        }
        var filteredFlights = flights.SelectMany(flightRoute => flightRoute.itineraries)
            .Where(flight => flight.prices.adult >= minPrice && flight.prices.adult <= maxPrice);
        if (!filteredFlights.Any())
        {
            return NotFound();
        }
        return Ok(filteredFlights);
    }
    
    //Choosing flights depending on given times
    [HttpGet("from/{from}/to/{to}/departure/{departure}/arrival/{arrival}")]
    public IActionResult GetFlightRoutes(string from, string to, DateTime departure, DateTime arrival)
    {
        var flights = _flightRoutes!.Where(flightRoute => flightRoute.departureDestination == from && flightRoute.arrivalDestination == to);
        if (!flights.Any())
        {
            return NotFound();
        }
        var filteredFlights = flights.SelectMany(flightRoute => flightRoute.itineraries)
            .Where(flight => flight.departureAt >= departure && flight.arrivalAt <= arrival);
        if (!filteredFlights.Any())
        {
            return NotFound();
        }
        return Ok(filteredFlights);
    }
    
    // User should be able to book a flight with Error checking for invalid bookings (Not enough seating, etc)
    [HttpPost("book")]
    public IActionResult BookFlight(Booking booking)
    {
        var flight = _flightRoutes!.SelectMany(flightRoute => flightRoute.itineraries)
            .FirstOrDefault(flight => flight.flight_id == booking.flight_id);
        if (flight == null)
        {
            return NotFound();
        }
        if (flight.availableSeats < booking.seats)
        {
            return BadRequest("Not enough seats available");
        }
        flight.availableSeats -= booking.seats;
        return Ok(booking);
    }
    
}
