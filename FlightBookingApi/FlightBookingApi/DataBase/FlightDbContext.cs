using FlightBookingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightBookingApi.DataBase;

public class FlightDbContext : DbContext
{
    public FlightDbContext(DbContextOptions<FlightDbContext> options) : base(options) { }
    
    public DbSet<Flight> Flights { get; set; }
    public DbSet<FlightRoute> FlightRoutes { get; set; }
    public DbSet<Price> Prices { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}