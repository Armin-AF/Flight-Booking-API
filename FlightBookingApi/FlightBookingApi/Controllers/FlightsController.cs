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
}