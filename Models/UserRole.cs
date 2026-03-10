using System;

namespace ECommerceAPI.Models
{
    public class UserRole
    {
        public int Id { get; set; }

        // FK → User
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // FK → Role
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
