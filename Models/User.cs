using System;
using System.Collections.Generic;

namespace ECommerceAPI.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";

        // ⭐ SOCIAL LOGIN SUPPORT
        public string? GoogleId { get; set; }
        public string? FacebookId { get; set; }
        public string? Provider { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}