using FlightInformation.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlightInformation.API.Data
{
    public class FlightDbContext : DbContext
    {
        public FlightDbContext(DbContextOptions<FlightDbContext> options) : base(options)
        {
            
        }
        
        public DbSet<Flight> Flights { get; set; }
        
        public DbSet<Status> Statuses { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Flight>()
                .Property(flight => flight.Status)
                .HasConversion<int>();

            modelBuilder.Entity<Status>()
                .HasData(new List<Status>()
                {
                    new Status() { Id = 1, Name = "Scheduled" },
                    new Status() { Id = 2, Name = "Delayed" },
                    new Status() { Id = 3, Name = "Cancelled" },
                    new Status() { Id = 4, Name = "InAir" },
                    new Status() { Id = 5, Name = "Landed" },
                });
        }
    }
}