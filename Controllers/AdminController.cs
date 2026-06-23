using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogisticaApp.Data;

namespace LogisticaApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userCount = await _context.Users.CountAsync();
            var shipmentCount = await _context.Shipments.CountAsync();
            var driverCount = await _context.Drivers.CountAsync();
            
            ViewBag.UserCount = userCount;
            ViewBag.ShipmentCount = shipmentCount;
            ViewBag.DriverCount = driverCount;
            
            return View();
        }
    }
}
