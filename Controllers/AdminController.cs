using Microsoft.AspNetCore.Mvc;
using LogisticaApp.Services;
using LogisticaApp.Data;
using ClosedXML.Excel;

namespace LogisticaApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly FinancialService _financialService;
        private readonly AppDbContext _context;

        public AdminController(FinancialService financialService, AppDbContext context)
        {
            _financialService = financialService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var startDate = DateTime.UtcNow.AddMonths(-1);
            var endDate = DateTime.UtcNow;

            var summary = await _financialService.GetFinancialSummaryAsync(startDate, endDate);
            return View(summary);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime startDate, DateTime endDate)
        {
            var summary = await _financialService.GetFinancialSummaryAsync(startDate, endDate);
            return View(summary);
        }

        public async Task<IActionResult> ExportToExcel(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.UtcNow.AddMonths(-1);
            endDate ??= DateTime.UtcNow;

            var summary = await _financialService.GetFinancialSummaryAsync(startDate, endDate);

            using (var workbook = new XLWorkbook())
            {
                // Resumen Financiero
                var ws1 = workbook.Worksheets.Add("Resumen Financiero");
                ws1.Cell("A1").Value = "RESUMEN FINANCIERO";
                ws1.Cell("A1").Style.Font.Bold = true;
                ws1.Cell("A1").Style.Font.FontSize = 14;

                ws1.Cell("A3").Value = "Período";
                ws1.Cell("B3").Value = $"{startDate:yyyy-MM-dd} a {endDate:yyyy-MM-dd}";

                ws1.Cell("A5").Value = "Métrica";
                ws1.Cell("B5").Value = "Valor";
                ws1.Row(5).Style.Font.Bold = true;

                ws1.Cell("A6").Value = "Ingresos Totales";
                ws1.Cell("B6").Value = summary.TotalRevenue;
                ws1.Cell("B6").Style.NumberFormat.Format = "$#,##0.00";

                ws1.Cell("A7").Value = "Costos Operativos";
                ws1.Cell("B7").Value = summary.OperatingCosts;
                ws1.Cell("B7").Style.NumberFormat.Format = "$#,##0.00";

                ws1.Cell("A8").Value = "Ganancia Neta";
                ws1.Cell("B8").Value = summary.Profit;
                ws1.Cell("B8").Style.NumberFormat.Format = "$#,##0.00";

                ws1.Cell("A9").Value = "Margen de Ganancia (%)";
                ws1.Cell("B9").Value = summary.ProfitMargin;
                ws1.Cell("B9").Style.NumberFormat.Format = "0.00%";

                ws1.Cell("A10").Value = "Total Envíos";
                ws1.Cell("B10").Value = summary.TotalShipments;

                ws1.Cell("A11").Value = "Valor Promedio Envío";
                ws1.Cell("B11").Value = summary.AverageShipmentValue;
                ws1.Cell("B11").Style.NumberFormat.Format = "$#,##0.00";

                ws1.Columns("A:B").AdjustToContents();

                // Ingresos por Ruta
                var ws2 = workbook.Worksheets.Add("Ingresos por Ruta");
                ws2.Cell("A1").Value = "INGRESOS POR RUTA";
                ws2.Cell("A1").Style.Font.Bold = true;

                ws2.Cell("A3").Value = "Ruta";
                ws2.Cell("B3").Value = "Ingresos";
                ws2.Cell("C3").Value = "Envíos";
                ws2.Row(3).Style.Font.Bold = true;

                int row = 4;
                foreach (var route in summary.RevenueByRoute)
                {
                    ws2.Cell($"A{row}").Value = route.RouteName;
                    ws2.Cell($"B{row}").Value = route.Revenue;
                    ws2.Cell($"B{row}").Style.NumberFormat.Format = "$#,##0.00";
                    ws2.Cell($"C{row}").Value = route.ShipmentCount;
                    row++;
                }

                ws2.Columns("A:C").AdjustToContents();

                // Ingresos por Cliente
                var ws3 = workbook.Worksheets.Add("Ingresos por Cliente");
                ws3.Cell("A1").Value = "INGRESOS POR CLIENTE";
                ws3.Cell("A1").Style.Font.Bold = true;

                ws3.Cell("A3").Value = "Cliente";
                ws3.Cell("B3").Value = "Ingresos";
                ws3.Cell("C3").Value = "Envíos";
                ws3.Row(3).Style.Font.Bold = true;

                row = 4;
                foreach (var client in summary.RevenueByClient)
                {
                    ws3.Cell($"A{row}").Value = client.ClientName;
                    ws3.Cell($"B{row}").Value = client.Revenue;
                    ws3.Cell($"B{row}").Style.NumberFormat.Format = "$#,##0.00";
                    ws3.Cell($"C{row}").Value = client.ShipmentCount;
                    row++;
                }

                ws3.Columns("A:C").AdjustToContents();

                var ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;

                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    $"Reporte_Financiero_{DateTime.UtcNow:yyyy-MM-dd}.xlsx");
            }
        }
    }
}
