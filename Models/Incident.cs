namespace LogisticaApp.Models
{
    public class Incident
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }
        public int DriverId { get; set; }
        public string IncidentType { get; set; }
        public string Description { get; set; }
        public string PhotoPath { get; set; }
        public byte[] PhotoData { get; set; }
        public DateTime CreatedAt { get; set; }
        public IncidentStatus Status { get; set; }
        public string ResolutionNotes { get; set; }
        public DateTime? ResolvedAt { get; set; }
        
        public Shipment Shipment { get; set; }
        public Driver Driver { get; set; }
    }

    public enum IncidentStatus
    {
        Reported,
        Investigating,
        Resolved,
        Closed
    }
}
