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
        private readonly AssignmentService _assignmentService;

        public ShipmentsController(AppDbContext context, EmailService emailService, AssignmentService assignmentService)
        {
            _context = context;
            _emailService = emailService;
            _assignmentService = assignmentService;
        }

        public async Task<IActionResult> Index()
        {
            var shipments = await _context.Shipments
                .Include(s => s.Client)
                .Include(s => s.Driver)
                .Include(s => s.Route)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            return View(shipments);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Routes = await _context.Routes.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Shipment shipment)
        {
            var clientId = HttpContext.Session.GetInt32("UserId");
            if (!clientId.HasValue)
                return Unauthorized();

            shipment.ClientId = clientId.Value;
            shipment.CreatedAt = DateTime.UtcNow;
            shipment.Status = ShipmentStatus.Created;

            _context.Add(shipment);
            await _context.SaveChangesAsync();

            var client = await _context.Users.FindAsync(clientId);
            if (client != null)
            {
                await _emailService.SendShipmentCreatedAsync(client.Email, client.Name, shipment.GuideNumber);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AssignSmartly(int id)
        {
            var result = await _assignmentService.AssignShipmentAsync(id);
            
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                TempData["ErrorMessage"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DriverWorkload()
        {
            var workload = await _assignmentService.GetDriverWorkloadAsync();
            return View(workload);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var shipment = await _context.Shipments.FindAsync(id);
            if (shipment == null)
                return NotFound();
            
            ViewBag.Routes = await _context.Routes.ToListAsync();
            return View(shipment);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Shipment shipment)
        {
            if (id != shipment.Id)
                return NotFound();

            try
            {
                _context.Update(shipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var shipment = await _context.Shipments
                .Include(s => s.Client)
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
    }
}
