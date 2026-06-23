using System;
using System.Collections.Generic;

namespace LogisticaApp.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        
        // Información del vehículo
        public string LicensePlate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        
        // Capacidad
        public decimal MaxWeightKg { get; set; }
        public decimal CurrentWeightKg { get; set; } = 0;
        
        // Mantenimiento
        public DateTime LastMaintenanceDate { get; set; }
        public DateTime NextMaintenanceDate { get; set; }
        
        // Estado
        public VehicleStatus Status { get; set; } = VehicleStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Relaciones
        public virtual ICollection<Driver> AssignedDrivers { get; set; } = new List<Driver>();
    }

    public enum VehicleStatus
    {
        Active = 1,
        Maintenance = 2,
        Inactive = 3
    }
}