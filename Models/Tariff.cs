using System;

namespace LogisticaApp.Models
{
    public class Tariff
    {
        public int Id { get; set; }
        
        // Precios base
        public decimal BasePrice { get; set; }
        public decimal PricePerKm { get; set; }
        public decimal PricePerKg { get; set; }
        
        // Recargos especiales
        public decimal UrgentSurcharge { get; set; }
        public decimal SpecialHandlingSurcharge { get; set; }
        
        // Descuentos
        public decimal LoyaltyDiscount { get; set; }
        public decimal BulkDiscount { get; set; }
        
        // Validez
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Auditoría
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }
}