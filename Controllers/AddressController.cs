using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using System.Security.Claims;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/address")]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public AddressController(ECommerceDbContext context)
        {
            _context = context;
        }

        // ================= GET ADDRESSES =================
        [HttpGet]
        public IActionResult Get()
        {
            int userId = GetUserId();

            var addresses = _context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedAt)
                .ToList();

            return Ok(addresses);
        }

        // ================= ADD =================
        [HttpPost]
        public IActionResult Add(Address address)
        {
            int userId = GetUserId();

            address.UserId = userId;
            address.CreatedAt = DateTime.UtcNow;

            // first address becomes default automatically
            if (!_context.Addresses.Any(a => a.UserId == userId))
                address.IsDefault = true;

            _context.Addresses.Add(address);
            _context.SaveChanges();

            return Ok(address);
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public IActionResult Update(int id, Address dto)
        {
            int userId = GetUserId();

            var address = _context.Addresses
                .FirstOrDefault(a => a.Id == id && a.UserId == userId);

            if (address == null)
                return NotFound();

            address.AddressTitle = dto.AddressTitle;
            address.FullName = dto.FullName;
            address.Phone = dto.Phone;
            address.AddressLine = dto.AddressLine;
            address.City = dto.City;
            address.Pincode = dto.Pincode;

            _context.SaveChanges();

            return Ok(address);
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            int userId = GetUserId();

            var address = _context.Addresses
                .FirstOrDefault(a => a.Id == id && a.UserId == userId);

            if (address == null)
                return NotFound();

            _context.Addresses.Remove(address);
            _context.SaveChanges();

            return Ok("Deleted");
        }

        // ================= SET DEFAULT =================
        [HttpPost("set-default/{id}")]
        public IActionResult SetDefault(int id)
        {
            int userId = GetUserId();

            var addresses = _context.Addresses
                .Where(a => a.UserId == userId)
                .ToList();

            foreach (var a in addresses)
                a.IsDefault = a.Id == id;

            _context.SaveChanges();

            return Ok("Default updated");
        }

        // ================= HELPER =================
        private int GetUserId()
        {
            return int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );
        }
    }
}
