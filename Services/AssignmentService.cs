using LogisticaApp.Data;
using LogisticaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticaApp.Services
{
    public class AssignmentService
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public AssignmentService(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<AssignmentResult> AssignShipmentAsync(int shipmentId)
        {
            var shipment = await _context.Shipments
                .Include(s => s.Client)
                .Include(s => s.Route)
                .FirstOrDefaultAsync(s => s.Id == shipmentId);

            if (shipment == null)
                return new AssignmentResult { Success = false, Message = "Envío no encontrado" };

            var availableDrivers = await GetAvailableDriversAsync(shipment);

            if (!availableDrivers.Any())
                return new AssignmentResult { Success = false, Message = "No hay conductores disponibles" };

            var bestDriver = SelectBestDriver(availableDrivers, shipment);

            shipment.DriverId = bestDriver.Id;
            shipment.Status = ShipmentStatus.Assigned;
            shipment.AssignedAt = DateTime.UtcNow;

            _context.Update(shipment);
            await _context.SaveChangesAsync();

            await _emailService.SendShipmentAssignedAsync(shipment.Client.Email, shipment.Client.Name, 
                shipment.GuideNumber, bestDriver.User.Name);

            return new AssignmentResult
            {
                Success = true,
                Message = $"Asignado a {bestDriver.User.Name}",
                DriverId = bestDriver.Id,
                DriverName = bestDriver.User.Name
            };
        }

        private async Task<List<Driver>> GetAvailableDriversAsync(Shipment shipment)
        {
            var drivers = await _context.Drivers
                .Include(d => d.User)
                .Include(d => d.Vehicle)
                .Where(d => d.Status == DriverStatus.Available)
                .ToListAsync();

            var availableDrivers = new List<Driver>();

            foreach (var driver in drivers)
            {
                var hasConflict = await _context.Shipments
                    .Where(s => s.DriverId == driver.Id
                        && s.Status != ShipmentStatus.Delivered
                        && s.Status != ShipmentStatus.Cancelled
                        && s.AssignedAt.HasValue)
                    .AnyAsync();

                if (!hasConflict)
                    availableDrivers.Add(driver);
            }

            return availableDrivers;
        }

        private Driver SelectBestDriver(List<Driver> drivers, Shipment shipment)
        {
            var driversWithStats = drivers.Select(d => new
            {
                Driver = d,
                ActiveDeliveries = _context.Shipments.Count(s => s.DriverId == d.Id 
                    && s.Status != ShipmentStatus.Delivered 
                    && s.Status != ShipmentStatus.Cancelled),
                AverageRating = d.AverageRating
            }).OrderBy(x => x.ActiveDeliveries)
              .ThenByDescending(x => x.AverageRating)
              .ToList();

            return driversWithStats.First().Driver;
        }

        public async Task<List<DriverWorkload>> GetDriverWorkloadAsync()
        {
            var drivers = await _context.Drivers
                .Include(d => d.User)
                .ToListAsync();

            var workloads = new List<DriverWorkload>();

            foreach (var driver in drivers)
            {
                var activeDeliveries = await _context.Shipments
                    .CountAsync(s => s.DriverId == driver.Id 
                        && s.Status != ShipmentStatus.Delivered 
                        && s.Status != ShipmentStatus.Cancelled);

                workloads.Add(new DriverWorkload
                {
                    DriverId = driver.Id,
                    DriverName = driver.User.Name,
                    ActiveDeliveries = activeDeliveries,
                    Status = driver.Status.ToString(),
                    Rating = driver.AverageRating
                });
            }

            return workloads.OrderBy(w => w.ActiveDeliveries).ToList();
        }
    }

    public class AssignmentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? DriverId { get; set; }
        public string DriverName { get; set; }
    }

    public class DriverWorkload
    {
        public int DriverId { get; set; }
        public string DriverName { get; set; }
        public int ActiveDeliveries { get; set; }
        public string Status { get; set; }
        public decimal Rating { get; set; }
    }
}
