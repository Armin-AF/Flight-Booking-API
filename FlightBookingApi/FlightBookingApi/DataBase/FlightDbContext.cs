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

        modelBuilder.Entity<Flight>()
            .HasMany(f => f.bookings)
            .WithOne(b => b.flight)
            .HasForeignKey(b => b.flight_id);

        modelBuilder.Entity<FlightRoute>()
            .HasMany(r => r.itineraries)
            .WithOne(f => f.route)
            .HasForeignKey(f => f.route_id);

        modelBuilder.Entity<Flight>()
            .HasOne(f => f.prices)
            .WithOne()
            .HasForeignKey<Price>(p => p.flight_id)
            .IsRequired();

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.flight)
            .WithMany(f => f.bookings)
            .HasForeignKey(b => b.flight_id);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.customer)
            .WithMany()
            .HasForeignKey(b => b.customer_id);

        modelBuilder.Entity<Price>()
            .HasKey(p => p.flight_id);
    }
}