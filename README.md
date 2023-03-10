# Flight Booking API

This is a project to build a flight booking API for a new flight booking company. The purpose of the project is to build a backend prototype that provides the required endpoints for a flight booking website.

## Background

A newly started flight booker company needs your help, and they need it quickly! They’re supposed to publish their application and business within two weeks but haven't even gotten started on the booking-page itself. They have the frontend part under control, that’s for an external consultant to fix for them, but they need you to fix the backend!

## Requirements

The API includes the following endpoints:

- Choosing two locations and getting all available flights between these locations
- Choosing flights depending on given times
- User should be able to book a flight
- Error checking for invalid bookings (Not enough seating, etc)
- Have flights with layovers. You should then connect existing flights with each other if a direct flight doesn't exist. For example, someone searches for Stockholm to Amsterdam. You don’t have any direct flight in your db for this, but you do have flights for Stockholm -> Oslo and Oslo -> Amsterdam. Then combine these two and present them as one flight, showing time for each flight and wait time between flights.
- Set a price-range in your search

## Implementation Details

- The project is implemented in C# using the ASP.NET Core framework.

- The data for the flights are stored in a JSON file provided with the project. The application reads this file at startup and loads the flight data into memory.

- The project has a BookingsController that handles the endpoints for the bookings. 

### The controller includes the following methods:

#### /customer: manages customer data
- GET: retrieves all customers
- GET {id}: retrieves a single customer by ID
- POST: creates a new customer
- PUT {id}: updates an existing customer
- DELETE {id}: deletes a customer
#### /flights: manages flight data
- GET: retrieves all flights
- GET {id}: retrieves a single flight by ID
- GET from/{from}/to/{to}: retrieves all flights from a given departure location to a given arrival location

## Getting Started
- Clone the repository to your local machine.
-Open the project in Rider or your preferred C# IDE.
- Build and run the project.
- The application will start running at http://localhost:5065.

Testing
The API endpoints can be tested using a tool such as Postman or Swagger.
