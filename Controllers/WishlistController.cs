using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using System.Security.Claims;

namespace ECommerceAPI.Controllers
{
    [Authorize] // 🔐 JWT PROTECTION
    [ApiController]
    [Route("api/[controller]")]
    public class WishlistController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public WishlistController(ECommerceDbContext context)
        {
            _context = context;
        }

        // 🔐 Helper: Get logged-in user email safely
        private string? GetUserEmail()
        {
            return User.FindFirstValue(ClaimTypes.Email);
        }

        // GET: api/wishlist
        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var userEmail = GetUserEmail();
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var items = await _context.WishlistItems
                .Where(w => w.UserEmail == userEmail)
                .ToListAsync();

            return Ok(items);
        }

        // POST: api/wishlist/{productId}
        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userEmail = GetUserEmail();
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var exists = await _context.WishlistItems
                .AnyAsync(w => w.ProductId == productId && w.UserEmail == userEmail);

            if (exists)
                return BadRequest("Product already in wishlist");

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound("Product not found");

            var item = new WishlistItem
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Image = product.ImageUrl ?? string.Empty, 
                UserEmail = userEmail                     
            };

            _context.WishlistItems.Add(item);
            await _context.SaveChangesAsync();

            return Ok(item);
        }

        // DELETE: api/wishlist/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromWishlist(int id)
        {
            var userEmail = GetUserEmail();
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.Id == id && w.UserEmail == userEmail);

            if (item == null)
                return NotFound();

            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
