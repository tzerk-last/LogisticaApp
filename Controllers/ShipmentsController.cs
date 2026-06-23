using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogisticaApp.Data;
using LogisticaApp.Models;
using LogisticaApp.Services;

namespace LogisticaApp.Controllers
{
    public class ShipmentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public ShipmentsController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            var shipments = await _context.Shipments
                .Include(s => s.Client)
                .Include(s => s.Driver)
                .Include(s => s.Route)
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
                shipment.Status = ShipmentStatus.Created;
                _context.Add(shipment);
                await _context.SaveChangesAsync();

                // Enviar email al cliente
                var client = await _context.Users.FindAsync(shipment.ClientId);
                if (client != null)
                {
                    await _emailService.SendShipmentCreatedAsync(client.Email, client.Name, shipment.GuideNumber);
                }

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Clients = _context.Users.Where(u => u.Role == UserRole.Client).ToList();
            ViewBag.Routes = _context.Routes.ToList();
            return View(shipment);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var shipment = await _context.Shipments
                .Include(s => s.Client)
                .Include(s => s.Driver)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shipment == null)
                return NotFound();

            ViewBag.Clients = _context.Users.Where(u => u.Role == UserRole.Client).ToList();
            ViewBag.Routes = _context.Routes.ToList();
            ViewBag.Drivers = _context.Drivers.Include(d => d.User).ToList();
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
                    var originalShipment = await _context.Shipments
                        .Include(s => s.Driver)
                        .Include(s => s.Client)
                        .FirstOrDefaultAsync(s => s.Id == id);

                    // Si cambió de estado, enviar notificación
                    if (originalShipment.Status != shipment.Status)
                    {
                        var client = originalShipment.Client;
                        if (shipment.Status == ShipmentStatus.Assigned && shipment.DriverId.HasValue)
                        {
                            var driver = await _context.Drivers.Include(d => d.User).FirstOrDefaultAsync(d => d.Id == shipment.DriverId);
                            if (client != null && driver != null)
                            {
                                await _emailService.SendShipmentAssignedAsync(client.Email, client.Name, shipment.GuideNumber, driver.User.Name);
                            }
                        }
                        else if (shipment.Status == ShipmentStatus.Delivered)
                        {
                            if (client != null)
                            {
                                await _emailService.SendDeliveryCompletedAsync(client.Email, client.Name, shipment.GuideNumber);
                            }
                        }
                    }

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
            ViewBag.Drivers = _context.Drivers.Include(d => d.User).ToList();
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
