using System;

namespace LogisticaApp.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        
        // Relación con envío
        public int ShipmentId { get; set; }
        public virtual Shipment Shipment { get; set; }
        
        // Información del conductor
        public int DriverId { get; set; }
        public virtual Driver Driver { get; set; }
        
        // Foto de comprobante
        public string PhotoPath { get; set; }
        public byte[] PhotoData { get; set; }
        
        // Datos extraídos por IA
        public string ReceiverName { get; set; }
        public string ReceiverDocumentId { get; set; }
        public DateTime DeliveryDate { get; set; }
        
        // Validación
        public AIValidationStatus ValidationStatus { get; set; } = AIValidationStatus.Pending;
        public decimal AIConfidence { get; set; }
        public string ValidationNotes { get; set; }
        
        // GPS
        public double DeliveryLatitude { get; set; }
        public double DeliveryLongitude { get; set; }
        
        // Firma
        public string ReceiverSignature { get; set; }
        
        // Timestamp
        public DateTime DeliveredAt { get; set; } = DateTime.Now;
    }

    public enum AIValidationStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        ManualReview = 4
    }
}