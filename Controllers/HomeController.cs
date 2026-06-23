using Microsoft.AspNetCore.Mvc;
using LogisticaApp.Models;
using LogisticaApp.Services;
using LogisticaApp.Data;
using Microsoft.EntityFrameworkCore;

namespace LogisticaApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AuthService _authService;
        private readonly AppDbContext _context;

        public HomeController(AuthService authService, AppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _authService.Login(model.Email, model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Email o contraseña incorrectos");
                return View(model);
            }

            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());
            HttpContext.Session.SetString("UserName", user.Name);

            return RedirectToAction("Dashboard");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Password != model.PasswordConfirm)
            {
                ModelState.AddModelError("", "Las contraseñas no coinciden");
                return View(model);
            }

            var user = _authService.Register(model.Name, model.Email, model.Phone, model.Password, model.Role);

            if (user == null)
            {
                ModelState.AddModelError("", "El email ya está registrado");
                return View(model);
            }

            return RedirectToAction("Login");
        }

        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            var userRole = HttpContext.Session.GetString("UserRole");

            // Pasar datos según el rol
            if (userRole == "Admin")
            {
                ViewBag.TotalUsers = _context.Users.Count();
                ViewBag.TotalShipments = _context.Shipments.Count();
                ViewBag.TotalDrivers = _context.Drivers.Count();
                ViewBag.TotalRoutes = _context.Routes.Count();
            }
            else if (userRole == "Client")
            {
                ViewBag.MyShipments = _context.Shipments.Where(s => s.ClientId == int.Parse(userId)).Count();
                ViewBag.DeliveredShipments = _context.Shipments.Where(s => s.ClientId == int.Parse(userId) && s.Status == ShipmentStatus.Delivered).Count();
                ViewBag.PendingShipments = _context.Shipments.Where(s => s.ClientId == int.Parse(userId) && s.Status != ShipmentStatus.Delivered).Count();
            }
            else if (userRole == "Driver")
            {
                var driver = _context.Drivers.FirstOrDefault(d => d.UserId == int.Parse(userId));
                ViewBag.TotalDeliveries = driver?.TotalDeliveries ?? 0;
                ViewBag.Rating = driver?.AverageRating ?? 0;
            }

            ViewBag.UserRole = userRole;
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
