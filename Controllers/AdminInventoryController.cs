using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Data;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/admin/inventory")]
    [Authorize(Roles = "Admin,Manager")]
    public class AdminInventoryController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public AdminInventoryController(ECommerceDbContext context)
        {
            _context = context;
        }

        // ================= ADD STOCK =================
        [HttpPut("{productId}/add")]
        public IActionResult AddStock(int productId, [FromBody] int amount)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
                return NotFound("Product not found");

            if (amount <= 0)
                return BadRequest("Amount must be positive");

            product.Stock += amount;
            product.Availability = product.Stock > 0;

            _context.SaveChanges();

            return Ok(new
            {
                product.ProductId,
                product.Name,
                product.Stock
            });
        }

        // ================= REMOVE STOCK =================
        [HttpPut("{productId}/remove")]
        public IActionResult RemoveStock(int productId, [FromBody] int amount)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
                return NotFound("Product not found");

            if (amount <= 0)
                return BadRequest("Amount must be positive");

            product.Stock -= amount;
            if (product.Stock < 0) product.Stock = 0;

            product.Availability = product.Stock > 0;

            _context.SaveChanges();

            return Ok(new
            {
                product.ProductId,
                product.Name,
                product.Stock
            });
        }
    }
}
