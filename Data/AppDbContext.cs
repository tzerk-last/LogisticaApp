using Microsoft.EntityFrameworkCore;
using LogisticaApp.Models;

namespace LogisticaApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Models.Route> Routes { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.Client)
                .WithMany(u => u.ShipmentsAsClient)
                .HasForeignKey(s => s.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.Driver)
                .WithMany(d => d.Shipments)
                .HasForeignKey(s => s.DriverId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.Route)
                .WithMany(r => r.Shipments)
                .HasForeignKey(s => s.RouteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Shipment)
                .WithOne(s => s.Delivery)
                .HasForeignKey<Delivery>(d => d.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Driver>()
                .HasOne(d => d.User)
                .WithOne(u => u.DriverProfile)
                .HasForeignKey<Driver>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Driver>()
                .HasOne(d => d.Vehicle)
                .WithMany(v => v.AssignedDrivers)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Driver)
                .WithMany(dr => dr.Deliveries)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Shipment)
                .WithMany()
                .HasForeignKey(i => i.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Driver)
                .WithMany()
                .HasForeignKey(i => i.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Shipment>()
                .HasIndex(s => s.GuideNumber)
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.LicensePlate)
                .IsUnique();
        }
    }
}
