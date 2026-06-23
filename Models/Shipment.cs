using System;
using System.ComponentModel.DataAnnotations;

namespace LogisticaApp.Models
{
    public class Shipment
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El número de guía es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El número de guía debe tener entre 3 y 50 caracteres")]
        public string GuideNumber { get; set; }
        
        [Required(ErrorMessage = "Debe seleccionar un cliente")]
        public int ClientId { get; set; }
        
        public int? DriverId { get; set; }
        
        [Required(ErrorMessage = "Debe seleccionar una ruta")]
        public int RouteId { get; set; }
        
        [Required(ErrorMessage = "El tipo de carga es obligatorio")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string CargoType { get; set; }
        
        [Required(ErrorMessage = "El peso es obligatorio")]
        [Range(0.1, 10000, ErrorMessage = "El peso debe estar entre 0.1 y 10000 kg")]
        public decimal Weight { get; set; }
        
        [StringLength(500, ErrorMessage = "Máximo 500 caracteres")]
        public string Description { get; set; }
        
        public ShipmentStatus Status { get; set; } = ShipmentStatus.Created;
        
        [Required(ErrorMessage = "El precio base es obligatorio")]
        [Range(0.01, 999999, ErrorMessage = "Precio inválido")]
        public decimal BasePrice { get; set; }
        
        [Range(0, 999999, ErrorMessage = "Costo total inválido")]
        public decimal TotalCost { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeliveredAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        
        // Relaciones
        public virtual User Client { get; set; }
        public virtual Driver Driver { get; set; }
        public virtual Route Route { get; set; }
        public virtual Delivery Delivery { get; set; }
    }
    
    public enum ShipmentStatus
    {
        Created = 1,
        Assigned = 2,
        InTransit = 3,
        Delivered = 4,
        Cancelled = 5,
        PendingReview = 6
    }
}
