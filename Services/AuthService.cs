using LogisticaApp.Data;
using LogisticaApp.Models;
using BCrypt.Net;

namespace LogisticaApp.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public User? Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            
            if (user == null)
                return null;

            // Verificar contraseña con BCrypt
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return user;
        }

        public User? Register(string name, string email, string phone, string password, UserRole role)
        {
            // Verificar si el email ya existe
            if (_context.Users.Any(u => u.Email == email))
                return null;

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Name = name,
                Email = email,
                Phone = phone,
                PasswordHash = hashedPassword,
                Role = role,
                DocumentNumber = "",
                Address = "",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }
    }
}
