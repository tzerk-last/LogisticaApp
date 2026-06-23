using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogisticaApp.Data;
using LogisticaApp.Models;
using LogisticaApp.Services;

namespace LogisticaApp.Controllers
{
    public class DeliveriesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly DeliveryValidationService _validationService;
        private readonly EmailService _emailService;

        public DeliveriesController(AppDbContext context, DeliveryValidationService validationService, EmailService emailService)
        {
            _context = context;
            _validationService = validationService;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            var deliveries = await _context.Deliveries
                .Include(d => d.Shipment)
                .Include(d => d.Driver)
                .ToListAsync();
            return View(deliveries);
        }

        public async Task<IActionResult> ValidateDelivery(int shipmentId)
        {
            var shipment = await _context.Shipments
                .Include(s => s.Client)
                .Include(s => s.Driver)
                .FirstOrDefaultAsync(s => s.Id == shipmentId);

            if (shipment == null)
                return NotFound();

            ViewBag.Shipment = shipment;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ValidateDelivery(int shipmentId, IFormFile photoFile, string receiverName, string receiverDocumentId)
        {
            if (photoFile == null || photoFile.Length == 0)
            {
                ModelState.AddModelError("", "Debes cargar una foto");
                return View();
            }

            var shipment = await _context.Shipments
                .Include(s => s.Client)
                .Include(s => s.Driver)
                .FirstOrDefaultAsync(s => s.Id == shipmentId);

            if (shipment == null)
                return NotFound();

            // Validar con IA
            var validationResult = await _validationService.ValidateDeliveryPhotoAsync(photoFile);

            // Guardar foto
            var photoPath = Path.Combine("uploads", "deliveries", Guid.NewGuid().ToString() + ".jpg");
            var fullPath = Path.Combine("wwwroot", photoPath);
            var directory = Path.GetDirectoryName(fullPath);
            if (directory != null)
                Directory.CreateDirectory(directory);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await photoFile.CopyToAsync(stream);
            }

            var driverId = shipment.DriverId ?? 0;

            var delivery = new Delivery
            {
                ShipmentId = shipmentId,
                DriverId = driverId,
                PhotoPath = photoPath,
                ReceiverName = receiverName,
                ReceiverDocumentId = receiverDocumentId,
                DeliveryDate = DateTime.UtcNow,
                ValidationStatus = validationResult.IsApproved ? AIValidationStatus.Approved : AIValidationStatus.Rejected,
                AIConfidence = validationResult.ConfidenceScore,
                ValidationNotes = validationResult.ValidationNotes ?? ""
            };

            _context.Add(delivery);

            // Actualizar estado del envío
            shipment.Status = ShipmentStatus.Delivered;
            shipment.DeliveredAt = DateTime.UtcNow;
            _context.Update(shipment);

            await _context.SaveChangesAsync();

            // Enviar notificación
            if (validationResult.IsApproved)
            {
                await _emailService.SendValidationResultAsync(shipment.Client.Email, shipment.Client.Name, shipment.GuideNumber, true);
            }
            else
            {
                await _emailService.SendValidationResultAsync(shipment.Client.Email, shipment.Client.Name, shipment.GuideNumber, false);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
