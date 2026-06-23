using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogisticaApp.Data;
using LogisticaApp.Models;
using LogisticaApp.Services;

namespace LogisticaApp.Controllers
{
    public class IncidentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public IncidentsController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            var incidents = await _context.Incidents
                .Include(i => i.Shipment)
                .Include(i => i.Driver)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
            return View(incidents);
        }

        public async Task<IActionResult> Create(int? shipmentId)
        {
            if (shipmentId.HasValue)
            {
                var shipment = await _context.Shipments
                    .Include(s => s.Driver)
                    .FirstOrDefaultAsync(s => s.Id == shipmentId);
                
                if (shipment != null)
                {
                    ViewBag.ShipmentId = shipmentId;
                    ViewBag.GuideNumber = shipment.GuideNumber;
                    ViewBag.DriverId = shipment.DriverId;
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Incident incident, IFormFile photoFile)
        {
            if (photoFile != null && photoFile.Length > 0)
            {
                var photoPath = Path.Combine("uploads", "incidents", Guid.NewGuid().ToString() + ".jpg");
                var fullPath = Path.Combine("wwwroot", photoPath);
                var directory = Path.GetDirectoryName(fullPath);
                if (directory != null)
                    Directory.CreateDirectory(directory);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await photoFile.CopyToAsync(stream);
                }

                incident.PhotoPath = photoPath;

                using (var memoryStream = new MemoryStream())
                {
                    await photoFile.CopyToAsync(memoryStream);
                    incident.PhotoData = memoryStream.ToArray();
                }
            }

            incident.CreatedAt = DateTime.UtcNow;
            incident.Status = Models.IncidentStatus.Reported;

            _context.Add(incident);
            await _context.SaveChangesAsync();

            // Notificar al operador
            var shipment = await _context.Shipments
                .Include(s => s.Client)
                .FirstOrDefaultAsync(s => s.Id == incident.ShipmentId);

            if (shipment?.Client != null)
            {
                await _emailService.SendIncidentReportedAsync(shipment.Client.Email, 
                    shipment.Client.Name, shipment.GuideNumber, incident.Description);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var incident = await _context.Incidents
                .Include(i => i.Shipment)
                .Include(i => i.Driver)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (incident == null)
                return NotFound();

            return View(incident);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var incident = await _context.Incidents.FindAsync(id);
            if (incident == null)
                return NotFound();
            return View(incident);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Incident incident)
        {
            if (id != incident.Id)
                return NotFound();

            try
            {
                _context.Update(incident);
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
            var incident = await _context.Incidents
                .Include(i => i.Shipment)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (incident == null)
                return NotFound();

            return View(incident);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var incident = await _context.Incidents.FindAsync(id);
            if (incident != null)
            {
                _context.Incidents.Remove(incident);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
