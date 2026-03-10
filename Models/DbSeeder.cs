using ECommerceAPI.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ECommerceAPI.Data
{
    public static class DbSeeder
    {
        public static void Seed(ECommerceDbContext context)
        {
            // 🔹 Seed roles
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { RoleName = "Admin" },
                    new Role { RoleName = "Manager" },
                    new Role { RoleName = "User" }
                );
                context.SaveChanges();
            }

            // 🔹 Create default admin user
            if (!context.Users.Any(u => u.Email == "admin@shop.com"))
            {
                var adminUser = new User
                {
                    Name = "Super Admin",
                    Email = "admin@shop.com",
                    PasswordHash = HashPassword("Admin@123"),
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(adminUser);
                context.SaveChanges();

                // Assign Admin role
                var adminRole = context.Roles.First(r => r.RoleName == "Admin");

                context.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.UserId,
                    RoleId = adminRole.Id
                });

                context.SaveChanges();
            }
        }

        // 🔐 Simple password hashing
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
