using System;
using System.Collections.Generic;

namespace LogisticaApp.Models
{
    public class Route
    {
        public int Id { get; set; }
        
        // Puntos de la ruta
        public string OriginAddress { get; set; }
        public double OriginLatitude { get; set; }
        public double OriginLongitude { get; set; }
        
        public string DestinationAddress { get; set; }
        public double DestinationLatitude { get; set; }
        public double DestinationLongitude { get; set; }
        
        // Cálculos
        public decimal DistanceKm { get; set; }
        public int EstimatedMinutes { get; set; }
        
        // Estado
        public RouteStatus Status { get; set; } = RouteStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }

        // Relaciones
        public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }

    public enum RouteStatus
    {
        Pending = 1,
        Active = 2,
        Completed = 3,
        Cancelled = 4
    }
}