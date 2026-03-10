using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using System.Security.Claims;

namespace ECommerceAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public CartController(ECommerceDbContext context)
        {
            _context = context;
        }

        // ================= GET USER ID FROM JWT =================
        private int GetUserId()
        {
            // try multiple claim names safely
            var claim =
                User.FindFirst("userId")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("sub")?.Value;

            if (claim == null)
                throw new UnauthorizedAccessException("UserId not found in token");

            return int.Parse(claim);
        }

        // ================= ADD TO CART =================
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddCartDto dto)
        {
            int userId = GetUserId();

            var existing = await _context.Carts
                .FirstOrDefaultAsync(c =>
                    c.UserId == userId &&
                    c.ProductId == dto.ProductId);

            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
            }
            else
            {
                _context.Carts.Add(new Cart
                {
                    UserId = userId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    IsSelected = true,
                    IsWishlisted = false,
                    CreatedAt = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Added to cart" });
        }

        // ================= GET CART =================
        [HttpGet]
        public async Task<IActionResult> GetMyCart()
        {
            int userId = GetUserId();

            var cart = await _context.Carts
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return Ok(cart);
        }

        // ================= UPDATE CART =================
        [HttpPut("{cartId}")]
        public async Task<IActionResult> UpdateCart(int cartId, [FromBody] UpdateCartDto dto)
        {
            var cart = await _context.Carts.FindAsync(cartId);

            if (cart == null)
                return NotFound();

            cart.Quantity = dto.Quantity;
            cart.IsSelected = dto.IsSelected;
            cart.IsWishlisted = dto.IsWishlisted;

            await _context.SaveChangesAsync();

            return Ok();
        }

        // ================= DELETE CART =================
        [HttpDelete("{cartId}")]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            var cart = await _context.Carts.FindAsync(cartId);

            if (cart == null)
                return NotFound();

            _context.Carts.Remove(cart);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }

    // ================= DTOs =================

    public class AddCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartDto
    {
        public int Quantity { get; set; }
        public bool IsSelected { get; set; }
        public bool IsWishlisted { get; set; }
    }
}
