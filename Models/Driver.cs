using System;
using System.Collections.Generic;

namespace LogisticaApp.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime LicenseExpiry { get; set; }
        public int? VehicleId { get; set; }
        public decimal AverageRating { get; set; } = 5.0m;
        public int TotalDeliveries { get; set; }
        public DriverStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public virtual User User { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
        public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();
        public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
    }

    public enum DriverStatus
    {
        Available,
        OnDelivery,
        OnBreak,
        Inactive
    }
}
