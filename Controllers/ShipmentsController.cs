using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogisticaApp.Data;
using LogisticaApp.Models;

namespace LogisticaApp.Controllers
{
    public class ShipmentsController : Controller
    {
        private readonly AppDbContext _context;

        public ShipmentsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var shipments = await _context.Shipments
                .Include(s => s.Client)
                .Include(s => s.Driver)
                .ToListAsync();
            return View(shipments);
        }

        public IActionResult Create()
        {
            ViewBag.Clients = _context.Users.Where(u => u.Role == UserRole.Client).ToList();
            ViewBag.Routes = _context.Routes.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Shipment shipment)
        {
            if (ModelState.IsValid)
            {
                shipment.CreatedAt = DateTime.UtcNow;
                shipment.TotalCost = shipment.BasePrice;
                _context.Add(shipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Clients = _context.Users.Where(u => u.Role == UserRole.Client).ToList();
            ViewBag.Routes = _context.Routes.ToList();
            return View(shipment);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var shipment = await _context.Shipments.FindAsync(id);
            if (shipment == null)
                return NotFound();

            ViewBag.Clients = _context.Users.Where(u => u.Role == UserRole.Client).ToList();
            ViewBag.Routes = _context.Routes.ToList();
            ViewBag.Drivers = _context.Drivers.ToList();
            return View(shipment);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Shipment shipment)
        {
            if (id != shipment.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shipment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShipmentExists(shipment.Id))
                        return NotFound();
                    throw;
                }
            }
            ViewBag.Clients = _context.Users.Where(u => u.Role == UserRole.Client).ToList();
            ViewBag.Routes = _context.Routes.ToList();
            ViewBag.Drivers = _context.Drivers.ToList();
            return View(shipment);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var shipment = await _context.Shipments
                .Include(s => s.Client)
                .Include(s => s.Driver)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shipment == null)
                return NotFound();

            return View(shipment);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shipment = await _context.Shipments.FindAsync(id);
            if (shipment != null)
            {
                _context.Shipments.Remove(shipment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ShipmentExists(int id)
        {
            return _context.Shipments.Any(e => e.Id == id);
        }
    }
}
