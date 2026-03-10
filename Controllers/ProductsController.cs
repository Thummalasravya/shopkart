using ECommerceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public ProductsController(ECommerceDbContext context)
        {
            _context = context;
        }

        ////////////////////////////////////////////////////////
        // PRODUCT LIST
        ////////////////////////////////////////////////////////

        [HttpGet]
        public async Task<IActionResult> GetProducts(
            string? search,
            int? categoryId,
            int page = 1,
            int pageSize = 8)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var query = _context.Products
                .Include(p => p.CategoryRef)
                .Where(p => p.IsEnabled && p.Availability)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    p.Description.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            var total = await query.CountAsync();

            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    productId = p.ProductId,
                    name = p.Name,
                    description = p.Description,
                    price = p.Price,
                    brand = p.Brand,
                    imageUrl = baseUrl + "/" + p.ImageUrl,
                    stockQuantity = p.StockQuantity,
                    categoryId = p.CategoryId,
                    category = p.CategoryRef != null ? p.CategoryRef.Name : "Unknown"
                })
                .ToListAsync();

            return Ok(new
            {
                total,
                page,
                pageSize,
                data = products
            });
        }

        ////////////////////////////////////////////////////////
        // SINGLE PRODUCT
        ////////////////////////////////////////////////////////

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetProduct(string slug)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            slug = slug.ToUpper();

            var product = await _context.Products
                .Include(p => p.CategoryRef)
                .Where(p => p.Slug.ToUpper() == slug)
                .Select(p => new
                {
                    productId = p.ProductId,
                    name = p.Name,
                    description = p.Description,
                    price = p.Price,
                    brand = p.Brand,
                    imageUrl = baseUrl + "/" + p.ImageUrl,
                    stockQuantity = p.StockQuantity,
                    categoryId = p.CategoryId,
                    category = p.CategoryRef != null ? p.CategoryRef.Name : "Unknown"
                })
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound("Product not found");

            return Ok(product);
        }
    }
}