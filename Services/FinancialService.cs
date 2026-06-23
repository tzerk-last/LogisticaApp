using LogisticaApp.Data;
using LogisticaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisticaApp.Services
{
    public class FinancialService
    {
        private readonly AppDbContext _context;

        public FinancialService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<FinancialSummary> GetFinancialSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate ??= DateTime.UtcNow.AddMonths(-1);
            endDate ??= DateTime.UtcNow;

            var shipments = await _context.Shipments
                .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate && s.Status == ShipmentStatus.Delivered)
                .ToListAsync();

            var totalRevenue = shipments.Sum(s => s.TotalCost);
            var totalShipments = shipments.Count;
            var averageShipmentValue = totalShipments > 0 ? totalRevenue / totalShipments : 0;

            // Costo operativo estimado (30% del ingreso)
            var operatingCosts = totalRevenue * 0.30m;
            var profit = totalRevenue - operatingCosts;
            var profitMargin = totalRevenue > 0 ? (profit / totalRevenue) * 100 : 0;

            // Por ruta
            var revenueByRoute = await _context.Shipments
                .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate && s.Status == ShipmentStatus.Delivered)
                .Include(s => s.Route)
                .GroupBy(s => s.RouteId)
                .Select(g => new RouteRevenue
                {
                    RouteId = g.Key,
                    RouteName = g.First().Route.DestinationAddress,
                    Revenue = g.Sum(s => s.TotalCost),
                    ShipmentCount = g.Count()
                })
                .OrderByDescending(r => r.Revenue)
                .ToListAsync();

            // Por cliente
            var revenueByClient = await _context.Shipments
                .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate && s.Status == ShipmentStatus.Delivered)
                .Include(s => s.Client)
                .GroupBy(s => s.ClientId)
                .Select(g => new ClientRevenue
                {
                    ClientId = g.Key,
                    ClientName = g.First().Client.Name,
                    Revenue = g.Sum(s => s.TotalCost),
                    ShipmentCount = g.Count()
                })
                .OrderByDescending(r => r.Revenue)
                .ToListAsync();

            return new FinancialSummary
            {
                TotalRevenue = totalRevenue,
                OperatingCosts = operatingCosts,
                Profit = profit,
                ProfitMargin = profitMargin,
                TotalShipments = totalShipments,
                AverageShipmentValue = averageShipmentValue,
                RevenueByRoute = revenueByRoute,
                RevenueByClient = revenueByClient,
                StartDate = startDate.Value,
                EndDate = endDate.Value
            };
        }
    }

    public class FinancialSummary
    {
        public decimal TotalRevenue { get; set; }
        public decimal OperatingCosts { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitMargin { get; set; }
        public int TotalShipments { get; set; }
        public decimal AverageShipmentValue { get; set; }
        public List<RouteRevenue> RevenueByRoute { get; set; } = new();
        public List<ClientRevenue> RevenueByClient { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class RouteRevenue
    {
        public int RouteId { get; set; }
        public string RouteName { get; set; }
        public decimal Revenue { get; set; }
        public int ShipmentCount { get; set; }
    }

    public class ClientRevenue
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public decimal Revenue { get; set; }
        public int ShipmentCount { get; set; }
    }
}
