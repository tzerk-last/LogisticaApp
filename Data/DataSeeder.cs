using LogisticaApp.Models;
using BCrypt.Net;

namespace LogisticaApp.Data
{
    public class DataSeeder
    {
        public static void SeedData(AppDbContext context)
        {
            // Si ya hay usuarios, no hacer nada
            if (context.Users.Any())
                return;

            var users = new List<User>
            {
                new User
                {
                    Name = "Admin User",
                    Email = "admin@logisticaapp.com",
                    Phone = "3001234567",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
                    Role = UserRole.Admin,
                    DocumentNumber = "123456789",
                    Address = "Medellín, Colombia",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Name = "Cliente Test",
                    Email = "cliente@logisticaapp.com",
                    Phone = "3109876543",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Cliente123"),
                    Role = UserRole.Client,
                    DocumentNumber = "987654321",
                    Address = "Bogotá, Colombia",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Name = "Conductor Test",
                    Email = "conductor@logisticaapp.com",
                    Phone = "3105555555",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Conductor123"),
                    Role = UserRole.Driver,
                    DocumentNumber = "555555555",
                    Address = "Cali, Colombia",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}
