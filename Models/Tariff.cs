namespace LogisticaApp.Models
{
    public class Tariff
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal MinDistance { get; set; }
        public decimal MaxDistance { get; set; }
        public decimal BasePrice { get; set; }
        public decimal PricePerKg { get; set; }
        public bool IsPriority { get; set; }
        public bool IsActive { get; set; } = true;
        public string UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
