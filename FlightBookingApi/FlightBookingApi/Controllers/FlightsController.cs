using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using FlightBookingApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightBookingApi.Controllers;

[ApiController]
[Route("[controller]")]
public class FlightsController : ControllerBase
{
    private readonly List<FlightRoute>? _flightRoutes;

    public FlightsController()
    {
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
        var flight = _flightRoutes!.FirstOrDefault(flightRoute => flightRoute.Id == id);
        if (flight == null)
        {
            return NotFound();
        }
        return Ok(flight);
    }
    
    [HttpGet("from/{from}/to/{to}")]
    public IActionResult GetFlightRoutes(string from, string to)
    {
        var flights = _flightRoutes!.Where(flightRoute => flightRoute.DepartureDestination == from && flightRoute.ArrivalDestination == to);
        if (flights == null)
        {
            return NotFound();
        }
        return Ok(flights);
    }
    
    [HttpPost("book")]
    public IActionResult BookFlight(Itinerary itinerary)
    {
        var flight = _flightRoutes!.FirstOrDefault(flightRoute => flightRoute.Id == itinerary.FlightId);
        if (flight == null)
        {
            return NotFound();
        }
        if (flight.Flights[0].AvailableSeats == 0)
        {
            return BadRequest("No seats available");
        }
        flight.Flights[0].AvailableSeats--;
        return Ok(flight);
    }
}