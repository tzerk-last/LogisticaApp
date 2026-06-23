using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogisticaApp.Data;
using LogisticaApp.Models;

namespace LogisticaApp.Controllers
{
    public class TariffsController : Controller
    {
        private readonly AppDbContext _context;

        public TariffsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tariffs = await _context.Tariffs.OrderBy(t => t.MinDistance).ToListAsync();
            return View(tariffs);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Tariff tariff)
        {
            if (ModelState.IsValid)
            {
                tariff.CreatedAt = DateTime.UtcNow;
                tariff.UpdatedAt = DateTime.UtcNow;
                
                _context.Add(tariff);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tariff);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var tariff = await _context.Tariffs.FindAsync(id);
            if (tariff == null)
                return NotFound();
            return View(tariff);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Tariff tariff)
        {
            if (id != tariff.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    tariff.UpdatedAt = DateTime.UtcNow;
                    _context.Update(tariff);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }
            return View(tariff);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var tariff = await _context.Tariffs.FindAsync(id);
            if (tariff == null)
                return NotFound();
            return View(tariff);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tariff = await _context.Tariffs.FindAsync(id);
            if (tariff != null)
            {
                _context.Tariffs.Remove(tariff);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
