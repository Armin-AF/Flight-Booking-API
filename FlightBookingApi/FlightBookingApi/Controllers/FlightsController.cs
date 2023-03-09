using Microsoft.AspNetCore.Mvc;
using FlightBookingApi.Models;

namespace FlightBookingApi.Controllers; 


[ApiController] 
[Route("[controller]")] 
public class FlightsController : ControllerBase 

{
    readonly List<FlightRoute>? _flightRoutes;

    readonly ILogger<FlightsController> _logger;
    
    
    public FlightsController(ILogger<FlightsController> logger)
    { 
        _logger = logger;

        var json = System.IO.File.ReadAllText("DataBase/data.json");
        _flightRoutes = System.Text.Json.JsonSerializer.Deserialize<List<FlightRoute>>(json); 
    } 
    
    [HttpGet]
    public IActionResult GetFlights()
    {
        return Ok(_flightRoutes);
    }
    
    [HttpGet("{id}")]
    public IActionResult GetFlight(string id)
    {
        var flight = _flightRoutes?.SelectMany(fr => fr.itineraries)
            .FirstOrDefault(it => it.flight_id == id);
        if (flight == null) return NotFound();
        return Ok(flight);
    }


    [HttpGet("from/{from}/to/{to}")]
    public IActionResult GetFlightRoutes(string from, string to)
    {
        var flights = _flightRoutes!.Where(flightRoute => flightRoute.departureDestination == from && flightRoute.arrivalDestination == to);
        IEnumerable<FlightRoute> flightRoutes = flights as FlightRoute[] ?? flights.ToArray();
        if (flightRoutes.Any() || _flightRoutes == null)
        {
            return Ok(flightRoutes.SelectMany(flightRoute => flightRoute.itineraries));
        }

        var departureFlights = _flightRoutes.Where(flightRoute => flightRoute.departureDestination == from)
            .SelectMany(flightRoute => flightRoute.itineraries);
        
        _logger.LogInformation("Departure flights: {departureFlights}", departureFlights);
            
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



    [HttpGet("from/{from}/to/{to}/minPrice/{minPrice}/maxPrice/{maxPrice}")]
    public IActionResult GetFlightRoutes(string from, string to, decimal minPrice, decimal maxPrice)
    {
        var flights = _flightRoutes!.Where(flightRoute => flightRoute.departureDestination == from && flightRoute.arrivalDestination == to);
        IEnumerable<FlightRoute> flightRoutes = flights as FlightRoute[] ?? flights.ToArray();
        if (flightRoutes.Any() || _flightRoutes == null)
        {
            return Ok(flightRoutes.SelectMany(flightRoute => flightRoute.itineraries));
        }
        
        var departureFlights = _flightRoutes.Where(flightRoute => flightRoute.departureDestination == from)
            .SelectMany(flightRoute => flightRoute.itineraries);
        
        _logger.LogInformation("Departure flights: {departureFlights}", departureFlights);
            
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


    
    [HttpGet("from/{from}/to/{to}/minPrice/{minPrice}/maxPrice/{maxPrice}/departure/{departure}/arrival/{arrival}")]
    public IActionResult GetFlightRoutes(string from, string to, decimal minPrice = 0, decimal maxPrice = decimal.MaxValue, DateTime? departure = null, DateTime? arrival = null)
    {
        var flights = _flightRoutes!.Where(flightRoute => flightRoute.departureDestination == from && flightRoute.arrivalDestination == to);
        IEnumerable<FlightRoute> flightRoutes = flights as FlightRoute[] ?? flights.ToArray();
        if (flightRoutes.Any() || _flightRoutes == null)
        {
            return Ok(flightRoutes.SelectMany(flightRoute => flightRoute.itineraries));
        }

        var departureFlights = _flightRoutes.Where(flightRoute => flightRoute.departureDestination == from)
            .SelectMany(flightRoute => flightRoute.itineraries);
        _logger.LogInformation("Departure flights: {departureFlights}", departureFlights);

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
        _logger.LogInformation("Flights with layovers: {flightsWithLayovers}", flightsWithLayovers);


        if (!flightsWithLayovers.Any())
        {
            return NotFound();
        }

        return Ok(flightsWithLayovers);
    }
}
