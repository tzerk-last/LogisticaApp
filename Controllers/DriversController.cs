using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogisticaApp.Data;
using LogisticaApp.Models;

namespace LogisticaApp.Controllers
{
    public class DriversController : Controller
    {
        private readonly AppDbContext _context;

        public DriversController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var drivers = await _context.Drivers
                .Include(d => d.User)
                .Include(d => d.Vehicle)
                .ToListAsync();
            return View(drivers);
        }

        public IActionResult Create()
        {
            ViewBag.Users = _context.Users.Where(u => u.Role == UserRole.Driver).ToList();
            ViewBag.Vehicles = _context.Vehicles.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Driver driver)
        {
            if (ModelState.IsValid)
            {
                _context.Add(driver);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = _context.Users.Where(u => u.Role == UserRole.Driver).ToList();
            ViewBag.Vehicles = _context.Vehicles.ToList();
            return View(driver);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var driver = await _context.Drivers
                .Include(d => d.User)
                .Include(d => d.Vehicle)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (driver == null)
                return NotFound();

            ViewBag.Users = _context.Users.Where(u => u.Role == UserRole.Driver).ToList();
            ViewBag.Vehicles = _context.Vehicles.ToList();
            return View(driver);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Driver driver)
        {
            if (id != driver.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(driver);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverExists(driver.Id))
                        return NotFound();
                    throw;
                }
            }
            ViewBag.Users = _context.Users.Where(u => u.Role == UserRole.Driver).ToList();
            ViewBag.Vehicles = _context.Vehicles.ToList();
            return View(driver);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var driver = await _context.Drivers
                .Include(d => d.User)
                .Include(d => d.Vehicle)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (driver == null)
                return NotFound();

            return View(driver);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver != null)
            {
                _context.Drivers.Remove(driver);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DriverExists(int id)
        {
            return _context.Drivers.Any(e => e.Id == id);
        }
    }
}
