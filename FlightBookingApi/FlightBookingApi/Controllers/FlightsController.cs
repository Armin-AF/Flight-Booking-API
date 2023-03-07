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
    IEnumerable<FlightRoute> flightRoutes = flights as FlightRoute[] ?? flights.ToArray();
    if (flightRoutes.Any() || _flightRoutes == null)
    {
        return Ok(flightRoutes.SelectMany(flightRoute => flightRoute.itineraries));
    }
    else
    {
        var departureFlights = _flightRoutes.Where(flightRoute => flightRoute.departureDestination == from)
            .SelectMany(flightRoute => flightRoute.itineraries);
        
        var flightsWithLayovers = (from departureFlight in departureFlights
                let arrivalFlights = _flightRoutes.Where(flightRoute => flightRoute.arrivalDestination == to)
                    .SelectMany(flightRoute => flightRoute.itineraries)
                from arrivalFlight in arrivalFlights
                let earliestArrival = departureFlight.arrivalAt
                let latestDeparture = arrivalFlight.departureAt
                let layoverDuration = (latestDeparture - earliestArrival)
                select new Flight
                {
                    flight_id = departureFlight.flight_id + "-" + arrivalFlight.flight_id,
                    departureAt = departureFlight.departureAt,
                    arrivalAt = arrivalFlight.arrivalAt,
                    availableSeats = Math.Min(departureFlight.availableSeats, arrivalFlight.availableSeats),
                    prices = new Price { currency = departureFlight.prices.currency, adult = departureFlight.prices.adult + arrivalFlight.prices.adult, child = departureFlight.prices.child + arrivalFlight.prices.child },
                    layoverDuration = layoverDuration
                })
            .OrderBy(f => f.layoverDuration)
            .Take(10)
            .ToList();


        if (!flightsWithLayovers.Any())
        {
            return NotFound();
        }

        return Ok(flightsWithLayovers);
    }
}



    [HttpGet("from/{from}/to/{to}/minPrice/{minPrice}/maxPrice/{maxPrice}")]
public IActionResult GetFlightRoutes(string from, string to, decimal minPrice, decimal maxPrice)
{
    var flights = _flightRoutes!.Where(flightRoute => flightRoute.departureDestination == from && flightRoute.arrivalDestination == to);
    IEnumerable<FlightRoute> flightRoutes = flights as FlightRoute[] ?? flights.ToArray();
    if (flightRoutes.Any() || _flightRoutes == null)
    {
        return Ok(flightRoutes.SelectMany(flightRoute => flightRoute.itineraries));
    }
    else
    {
        var departureFlights = _flightRoutes.Where(flightRoute => flightRoute.departureDestination == from)
            .SelectMany(flightRoute => flightRoute.itineraries);
        
        var flightsWithLayovers = (from departureFlight in departureFlights
                let arrivalFlights = _flightRoutes.Where(flightRoute => flightRoute.arrivalDestination == to)
                    .SelectMany(flightRoute => flightRoute.itineraries)
                from arrivalFlight in arrivalFlights
                let earliestArrival = departureFlight.arrivalAt
                let latestDeparture = arrivalFlight.departureAt
                let layoverDuration = (latestDeparture - earliestArrival)
                let totalFlightPrice = departureFlight.prices.adult + arrivalFlight.prices.adult
                where totalFlightPrice >= minPrice && totalFlightPrice <= maxPrice
                select new Flight
                {
                    flight_id = departureFlight.flight_id + "-" + arrivalFlight.flight_id,
                    departureAt = departureFlight.departureAt,
                    arrivalAt = arrivalFlight.arrivalAt,
                    availableSeats = Math.Min(departureFlight.availableSeats, arrivalFlight.availableSeats),
                    prices = new Price { currency = departureFlight.prices.currency, adult = totalFlightPrice, child = departureFlight.prices.child + arrivalFlight.prices.child },
                    layoverDuration = layoverDuration
                })
            .OrderBy(f => f.layoverDuration)
            .Take(10)
            .ToList();


        if (!flightsWithLayovers.Any())
        {
            return NotFound();
        }

        return Ok(flightsWithLayovers);
    }
}


    
    [HttpGet("from/{from}/to/{to}/minPrice/{minPrice}/maxPrice/{maxPrice}/departure/{departure}/arrival/{arrival}")]
public IActionResult GetFlightRoutes(string from, string to, decimal minPrice = 0, decimal maxPrice = decimal.MaxValue, DateTime? departure = null, DateTime? arrival = null)
{
    var flights = _flightRoutes!.Where(flightRoute => flightRoute.departureDestination == from && flightRoute.arrivalDestination == to);
    IEnumerable<FlightRoute> flightRoutes = flights as FlightRoute[] ?? flights.ToArray();
    if (flightRoutes.Any() || _flightRoutes == null)
    {
        return Ok(flightRoutes.SelectMany(flightRoute => flightRoute.itineraries));
    }
    else
    {
        var departureFlights = _flightRoutes.Where(flightRoute => flightRoute.departureDestination == from)
            .SelectMany(flightRoute => flightRoute.itineraries);

        var flightsWithLayovers = (from departureFlight in departureFlights
                let arrivalFlights = _flightRoutes.Where(flightRoute => flightRoute.arrivalDestination == to)
                    .SelectMany(flightRoute => flightRoute.itineraries)
                from arrivalFlight in arrivalFlights
                let earliestArrival = departureFlight.arrivalAt
                let latestDeparture = arrivalFlight.departureAt
                let layoverDuration = (latestDeparture - earliestArrival)
                where departureFlight.prices.adult + arrivalFlight.prices.adult >= minPrice && departureFlight.prices.adult + arrivalFlight.prices.adult <= maxPrice
                    && (!departure.HasValue || departureFlight.departureAt >= departure.Value) && (!arrival.HasValue || arrivalFlight.arrivalAt <= arrival.Value)
                select new Flight
                {
                    flight_id = departureFlight.flight_id + "-" + arrivalFlight.flight_id,
                    departureAt = departureFlight.departureAt,
                    arrivalAt = arrivalFlight.arrivalAt,
                    availableSeats = Math.Min(departureFlight.availableSeats, arrivalFlight.availableSeats),
                    prices = new Price { currency = departureFlight.prices.currency, adult = departureFlight.prices.adult + arrivalFlight.prices.adult, child = departureFlight.prices.child + arrivalFlight.prices.child },
                    layoverDuration = layoverDuration
                })
            .OrderBy(f => f.layoverDuration)
            .Take(10)
            .ToList();


        if (!flightsWithLayovers.Any())
        {
            return NotFound();
        }

        return Ok(flightsWithLayovers);
    }
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
    
    // User should be able to cancel a booking
    [HttpDelete("cancel")]
    public IActionResult CancelFlight(Booking booking)
    {
        var flight = _flightRoutes!.SelectMany(flightRoute => flightRoute.itineraries)
            .FirstOrDefault(flight => flight.flight_id == booking.flight_id);
        if (flight == null)
        {
            return NotFound();
        }
        flight.availableSeats += booking.seats;
        return Ok(booking);
    }
}
