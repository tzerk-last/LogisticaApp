using System;
using System.Collections.Generic;

namespace LogisticaApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        public string DocumentNumber { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Relaciones
        public virtual ICollection<Shipment> ShipmentsAsClient { get; set; } = new List<Shipment>();
        public virtual Driver DriverProfile { get; set; }
    }

    public enum UserRole
    {
        Client = 1,
        Driver = 2,
        Operator = 3,
        Admin = 4
    }
}