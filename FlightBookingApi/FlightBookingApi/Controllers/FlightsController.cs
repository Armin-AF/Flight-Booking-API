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
    
    [HttpGet("from/{from}/to/{to}")]
    public IActionResult GetFlightRoutes(string from, string to)
    {
        var flights = _flightRoutes!.Where(flightRoute => flightRoute.departureDestination == from && flightRoute.arrivalDestination == to);
        if (!flights.Any())
        {
            return NotFound();
        }
        return Ok(flights);
    }
}
