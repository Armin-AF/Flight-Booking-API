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
        var directFlights = _flightRoutes
            .Where(flightRoute => flightRoute.departureDestination == from && flightRoute.arrivalDestination == to)
            .SelectMany(flightRoute => flightRoute.itineraries)
            .ToList();

        if (directFlights.Any())
        {
            return Ok(directFlights);
        }

        var departureFlights = _flightRoutes
            .Where(flightRoute => flightRoute.departureDestination == from)
            .SelectMany(flightRoute => flightRoute.itineraries)
            .ToList();

        var arrivalFlights = _flightRoutes
            .Where(flightRoute => flightRoute.arrivalDestination == to)
            .SelectMany(flightRoute => flightRoute.itineraries)
            .ToList();

        var flightsWithLayovers = (from departureFlight in departureFlights
                                   from arrivalFlight in arrivalFlights
                                   where departureFlight.arrivalAt <= arrivalFlight.departureAt
                                   let layoverDuration = (arrivalFlight.departureAt - departureFlight.arrivalAt)
                                   orderby layoverDuration
                                   select new Flight
                                   {
                                       flight_id = departureFlight.flight_id + "-" + arrivalFlight.flight_id,
                                       departureAt = departureFlight.departureAt,
                                       arrivalAt = arrivalFlight.arrivalAt,
                                       availableSeats = Math.Min(departureFlight.availableSeats, arrivalFlight.availableSeats),
                                       prices = new Price { currency = departureFlight.prices.currency, adult = departureFlight.prices.adult + arrivalFlight.prices.adult, child = departureFlight.prices.child + arrivalFlight.prices.child },
                                       layoverDuration = layoverDuration
                                   })
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
        var flightRoutes = _flightRoutes?
            .Where(flightRoute => flightRoute.departureDestination == from && flightRoute.arrivalDestination == to);

        if (flightRoutes == null || !flightRoutes.Any())
        {
            var departureFlights = _flightRoutes
                ?.Where(flightRoute => flightRoute.departureDestination == from)
                .SelectMany(flightRoute => flightRoute.itineraries);

            if (departureFlights == null)
            {
                return NotFound();
            }

            var flightsWithLayovers = (from departureFlight in departureFlights
                                       let arrivalFlights = _flightRoutes.Where(flightRoute => flightRoute.arrivalDestination == to)
                                           .SelectMany(flightRoute => flightRoute.itineraries)
                                       from arrivalFlight in arrivalFlights
                                       let earliestArrival = departureFlight.arrivalAt
                                       let latestDeparture = arrivalFlight.departureAt
                                       let layoverDuration = latestDeparture - earliestArrival
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
        else
        {
            var flights = flightRoutes.SelectMany(flightRoute => flightRoute.itineraries)
                .Where(flight => flight.prices.adult >= minPrice && flight.prices.adult <= maxPrice)
                .ToList();

            if (!flights.Any())
            {
                return NotFound();
            }

            return Ok(flights);
        }
    }

    
    [HttpGet("from/{from}/to/{to}/minPrice/{minPrice}/maxPrice/{maxPrice}/departure/{departure?}/arrival/{arrival?}")]
    public IActionResult GetFlightRoutes(string from, string to, decimal minPrice = 0, decimal maxPrice = decimal.MaxValue, DateTime? departure = null, DateTime? arrival = null)
    {
        var flights = _flightRoutes!.Where(flightRoute => flightRoute.departureDestination == from && flightRoute.arrivalDestination == to);
        if (!flights.Any())
        {
            return NotFound();
        }

        var departureFlights = _flightRoutes.Where(flightRoute => flightRoute.departureDestination == from)
            .SelectMany(flightRoute => flightRoute.itineraries);

        var flightsWithLayovers = departureFlights.SelectMany(departureFlight =>
        {
            var arrivalFlights = _flightRoutes.Where(flightRoute => flightRoute.arrivalDestination == to)
                .SelectMany(flightRoute => flightRoute.itineraries);

            var validArrivalFlights = arrivalFlights
                .Where(arrivalFlight => arrivalFlight.departureAt > departureFlight.arrivalAt &&
                                        (!arrival.HasValue || arrivalFlight.arrivalAt <= arrival.Value));

            return validArrivalFlights.Select(arrivalFlight =>
            {
                var totalFlightPrice = departureFlight.prices.adult + arrivalFlight.prices.adult;

                if (totalFlightPrice < minPrice || totalFlightPrice > maxPrice)
                {
                    return null;
                }

                var layoverDuration = arrivalFlight.departureAt - departureFlight.arrivalAt;
                if (layoverDuration < TimeSpan.Zero)
                {
                    return null;
                }

                if (departure.HasValue && departureFlight.departureAt < departure.Value)
                {
                    return null;
                }

                return new Flight
                {
                    flight_id = departureFlight.flight_id + "-" + arrivalFlight.flight_id,
                    departureAt = departureFlight.departureAt,
                    arrivalAt = arrivalFlight.arrivalAt,
                    availableSeats = Math.Min(departureFlight.availableSeats, arrivalFlight.availableSeats),
                    prices = new Price
                    {
                        currency = departureFlight.prices.currency,
                        adult = totalFlightPrice,
                        child = departureFlight.prices.child + arrivalFlight.prices.child
                    },
                    layoverDuration = layoverDuration
                };
            });
        })
        .Where(f => f != null)
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
