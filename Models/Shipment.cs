namespace LogisticaApp.Models
{
    public class Shipment
    {
        public int Id { get; set; }
        public string GuideNumber { get; set; }
        public int ClientId { get; set; }
        public int? DriverId { get; set; }
        public int RouteId { get; set; }
        public int? DeliveryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        
        public decimal Weight { get; set; }
        public string CargoType { get; set; }
        public string Description { get; set; }
        public decimal TotalCost { get; set; }
        
        public ShipmentStatus Status { get; set; }
        
        public User Client { get; set; }
        public Driver Driver { get; set; }
        public Route Route { get; set; }
        public Delivery Delivery { get; set; }
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
