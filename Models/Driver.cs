using System;
using System.Collections.Generic;

namespace LogisticaApp.Models
{
    public class Driver
    {
        public int Id { get; set; }
        
        // Relación con User
        public int UserId { get; set; }
        public virtual User User { get; set; }
        
        // Información del conductor
        public string LicenseNumber { get; set; }
        public DateTime LicenseExpiry { get; set; }
        public int? VehicleId { get; set; }
        
        // Calificación
        public decimal AverageRating { get; set; } = 5.0m;
        public int TotalDeliveries { get; set; } = 0;
        public int SuccessfulDeliveries { get; set; } = 0;

        // Estado
        public DriverStatus Status { get; set; } = DriverStatus.Available;
        
        // Coordenadas GPS actuales
        public double? CurrentLatitude { get; set; }
        public double? CurrentLongitude { get; set; }
        public DateTime? LastLocationUpdate { get; set; }

        // Relaciones
        public virtual Vehicle Vehicle { get; set; }
        public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
        public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
    }

    public enum DriverStatus
    {
        Available = 1,
        OnDelivery = 2,
        OnBreak = 3,
        Inactive = 4
    }
}