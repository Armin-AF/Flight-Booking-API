using FlightBookingApi.DataBase;
using FlightBookingApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Newtonsoft.Json;

namespace FlightBookingApi;

public class SeedData : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Read data from JSON file
        string json = File.ReadAllText("data.json");
        List<FlightRoute> flightRoutes = JsonConvert.DeserializeObject<List<FlightRoute>>(json);

        // Create database context
        var optionsBuilder = new DbContextOptionsBuilder<FlightDbContext>();
        optionsBuilder.UseSqlServer("your_connection_string_here");
        var dbContext = new FlightDbContext(optionsBuilder.Options);

        // Add each FlightRoute to the DbSet and save changes
        foreach (var flightRoute in flightRoutes)
        {
            dbContext.FlightRoutes.Add(flightRoute);
        }
        dbContext.SaveChanges();
    }
}