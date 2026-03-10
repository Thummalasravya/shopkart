using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.DTOs;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public AdminUsersController(ECommerceDbContext context)
        {
            _context = context;
        }

        ///////////////////////////////////////////////////////
        /// ADD USER WITH ROLE DROPDOWN
        ///////////////////////////////////////////////////////

        [HttpPost]
        public IActionResult AddUser([FromBody] RegisterDto dto)
        {
            var exists = _context.Users
                .Any(u => u.Email == dto.Email);

            if (exists)
                return BadRequest(new
                {
                    message = "User already exists"
                });


            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };


            _context.Users.Add(user);

            _context.SaveChanges();


            //////////////////////////////////////////////////
            // ⭐ Assign Role From Dropdown
            //////////////////////////////////////////////////

            var role = _context.Roles
                .FirstOrDefault(r => r.RoleName == dto.Role);


            // If role not found → default User

            if (role == null)
            {
                role = _context.Roles
                    .First(r => r.RoleName == "User");
            }


            _context.UserRoles.Add(
                new UserRole
                {
                    UserId = user.UserId,
                    RoleId = role.Id
                });


            _context.SaveChanges();


            return Ok(new
            {
                message = "User Added Successfully"
            });
        }



        ///////////////////////////////////////////////////////
        /// GET ALL USERS
        ///////////////////////////////////////////////////////

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Select(u => new
                {
                    userId = u.UserId,
                    name = u.Name,
                    email = u.Email,
                    roles = u.UserRoles
                        .Select(r => r.Role.RoleName)
                        .ToList()
                })
                .ToList();

            return Ok(users);
        }



        ///////////////////////////////////////////////////////
        /// GET USER BY ID
        ///////////////////////////////////////////////////////

        [HttpGet("{userId}")]
        public IActionResult GetUserById(int userId)
        {
            var user = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => u.UserId == userId)
                .Select(u => new
                {
                    userId = u.UserId,
                    name = u.Name,
                    email = u.Email,
                    roles = u.UserRoles
                        .Select(r => r.Role.RoleName)
                        .ToList()
                })
                .FirstOrDefault();

            if (user == null)
                return NotFound(new
                {
                    message = "User not found"
                });

            return Ok(user);
        }



        ///////////////////////////////////////////////////////
        /// UPDATE USER
        ///////////////////////////////////////////////////////

        [HttpPut("{userId}")]
        public IActionResult UpdateUser(
            int userId,
            [FromBody] User updatedUser)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                return NotFound(new
                {
                    message = "User not found"
                });

            user.Name = updatedUser.Name;

            user.Email = updatedUser.Email;

            _context.SaveChanges();

            return Ok(new
            {
                message = "User updated successfully"
            });
        }



        ///////////////////////////////////////////////////////
        /// ASSIGN ROLE
        ///////////////////////////////////////////////////////

        [HttpPost("{userId}/role/{roleName}")]
        public IActionResult AssignRole(
            int userId,
            string roleName)
        {
            var user =
                _context.Users.Find(userId);

            var role =
                _context.Roles
                .FirstOrDefault(
                    r => r.RoleName == roleName
                );

            if (user == null)
                return NotFound(new
                {
                    message = "User not found"
                });

            if (role == null)
                return NotFound(new
                {
                    message = "Role not found"
                });

            var exists =
                _context.UserRoles
                .Any(ur =>
                    ur.UserId == userId &&
                    ur.RoleId == role.Id);

            if (exists)
                return Ok(new
                {
                    message = "User already has this role"
                });


            _context.UserRoles.Add(
                new UserRole
                {
                    UserId = userId,
                    RoleId = role.Id
                });


            _context.SaveChanges();

            return Ok(new
            {
                message = "Role assigned successfully"
            });
        }



        ///////////////////////////////////////////////////////
        /// REMOVE ROLE
        ///////////////////////////////////////////////////////

        [HttpDelete("{userId}/role/{roleName}")]
        public IActionResult RemoveRole(
            int userId,
            string roleName)
        {
            var role =
                _context.Roles
                .FirstOrDefault(
                    r => r.RoleName == roleName
                );

            if (role == null)
                return NotFound(new
                {
                    message = "Role not found"
                });

            var userRole =
                _context.UserRoles
                .FirstOrDefault(ur =>
                    ur.UserId == userId &&
                    ur.RoleId == role.Id);

            if (userRole == null)
                return NotFound(new
                {
                    message = "User does not have this role"
                });


            _context.UserRoles.Remove(userRole);

            _context.SaveChanges();

            return Ok(new
            {
                message = "Role removed successfully"
            });
        }



        ///////////////////////////////////////////////////////
        /// DELETE USER
        ///////////////////////////////////////////////////////

        [HttpDelete("{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            var user =
                _context.Users
                .FirstOrDefault(u =>
                    u.UserId == userId);

            if (user == null)
                return NotFound(new
                {
                    message = "User not found"
                });


            var roles =
                _context.UserRoles
                .Where(x => x.UserId == userId)
                .ToList();


            _context.UserRoles.RemoveRange(roles);

            _context.Users.Remove(user);

            _context.SaveChanges();

            return Ok(new
            {
                message = "User deleted successfully"
            });
        }



        ///////////////////////////////////////////////////////
        /// PASSWORD HASH
        ///////////////////////////////////////////////////////

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();

            var bytes =
                Encoding.UTF8.GetBytes(password);

            var hash =
                sha.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

    }
}