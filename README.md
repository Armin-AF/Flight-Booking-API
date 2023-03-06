# Flight Booker API

This project is a flight booking API that provides users with information on available flights, prices, and booking information. It has been built for a newly started flight booker company that needs to quickly publish their application and business within two weeks.

## Getting Started

To get started with this project, clone the repository to your local machine and run the following commands:

```csharp
npm install
npm start
```

This will install all the required dependencies and start the server.

## API Endpoints

The API endpoints that are available for use include:

- GET /flights - This endpoint retrieves all available flights.
- GET /flights/:id - This endpoint retrieves a specific flight by ID.
- GET /flights/search - This endpoint allows the user to search for flights between two locations and within a given time frame.
- POST /bookings - This endpoint allows the user to book a flight.
- GET /bookings/:id - This endpoint retrieves a specific booking by ID.

## Database

The project uses an in-memory database to store flight and booking information. This means that all data is lost when the server is restarted. The flights data is stored in the flights.json file, which is loaded into the database when the server starts up.

## Error Checking

The API includes error checking for invalid bookings, such as not enough seating.

## Connecting Flights

The API also supports flights with layovers. If a direct flight is not available, the API will combine existing flights to create a connection between two locations.

## Price Range

The user can set a price range when searching for flights.

## Authentication

The API does not currently include authentication. However, it is recommended that authentication be added in the future, either via a login or a booking code.

## Testing

To test the API endpoints, you can use Swagger or Postman to check the validity of your endpoints and functionality of your application.

## Conclusion

This project provides a fully functioning flight booking API prototype that can be shown on your portfolio. It includes all required features and can be easily expanded upon in the future.
