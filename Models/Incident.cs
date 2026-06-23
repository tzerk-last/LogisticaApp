using System;

namespace LogisticaApp.Models
{
    public class Incident
    {
        public int Id { get; set; }
        
        // Relaciones
        public int ShipmentId { get; set; }
        public virtual Shipment Shipment { get; set; }
        
        public int DriverId { get; set; }
        public virtual Driver Driver { get; set; }
        
        // Tipo de incidencia
        public IncidentType IncidentType { get; set; }
        
        // Descripción del problema
        public string Description { get; set; }
        
        // Fotos de evidencia (opcional)
        public string PhotoPath { get; set; }
        public byte[] PhotoData { get; set; }
        
        // Ubicación
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        
        // Estado
        public IncidentStatus Status { get; set; } = IncidentStatus.Reported;
        
        // Resolución
        public string ResolutionNotes { get; set; }
        public DateTime ReportedAt { get; set; } = DateTime.Now;
        public DateTime? ResolvedAt { get; set; }
    }

    public enum IncidentType
    {
        IncorrectAddress = 1,
        RecipientAbsent = 2,
        VehicleBreakdown = 3,
        DamagedCargo = 4,
        Accident = 5,
        SecurityIssue = 6,
        Other = 7
    }

    public enum IncidentStatus
    {
        Reported = 1,
        InProgress = 2,
        Resolved = 3,
        Escalated = 4
    }
}