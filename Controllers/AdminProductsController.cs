using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/admin/products")]
    [AllowAnonymous] 
    public class AdminProductsController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public AdminProductsController(ECommerceDbContext context)
        {
            _context = context;
        }

        //////////////////////////////////////////////////
        // GET ALL PRODUCTS
        //////////////////////////////////////////////////
        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Products
                .Select(p => new
                {
                    p.ProductId,
                    p.Name,
                    p.Price,
                    p.ImageUrl,
                    p.StockQuantity,
                    p.CategoryId,
                    p.Brand,
                    p.CreatedAt
                })
                .ToList();

            return Ok(products);
        }

        //////////////////////////////////////////////////
        // GET PRODUCT BY ID
        //////////////////////////////////////////////////
        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _context.Products
                .Where(p => p.ProductId == id)
                .Select(p => new
                {
                    p.ProductId,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.ImageUrl,
                    p.StockQuantity,
                    p.CategoryId,
                    p.Brand,
                    p.IsEnabled,
                    p.IsExchangeable,
                    p.IsRefundable,
                    p.CreatedAt
                })
                .FirstOrDefault();

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        //////////////////////////////////////////////////
        // CREATE PRODUCT
        //////////////////////////////////////////////////
        [HttpPost]
        public IActionResult Create(ProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                Brand = dto.Brand,
                ImageUrl = dto.ImageUrl,
                StockQuantity = dto.StockQuantity,
                Stock = dto.StockQuantity,
                IsEnabled = dto.IsEnabled,
                IsExchangeable = dto.IsExchangeable,
                IsRefundable = dto.IsRefundable,
                Availability = dto.IsEnabled,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            if (dto.Tags != null && dto.Tags.Any())
            {
                foreach (var tagValue in dto.Tags)
                {
                    var tag = new Tag
                    {
                        ProductId = product.ProductId,
                        Value = tagValue
                    };

                    _context.Tags.Add(tag);
                }

                _context.SaveChanges();
            }

            return Ok(product);
        }

        //////////////////////////////////////////////////
        // UPDATE PRODUCT  🔥 (THIS FIXES YOUR 405)
        //////////////////////////////////////////////////
        [HttpPut("{id}")]
        public IActionResult Update(int id, ProductDto dto)
        {
            var product = _context.Products
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
                return NotFound();

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;
            product.Brand = dto.Brand;
            product.ImageUrl = dto.ImageUrl;
            product.StockQuantity = dto.StockQuantity;
            product.Stock = dto.StockQuantity;
            product.IsEnabled = dto.IsEnabled;
            product.IsExchangeable = dto.IsExchangeable;
            product.IsRefundable = dto.IsRefundable;

            _context.SaveChanges();

            return Ok(new { message = "Product updated successfully" });
        }

        //////////////////////////////////////////////////
        // DELETE PRODUCT
        //////////////////////////////////////////////////
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();

            return Ok(new { message = "Product Deleted Successfully" });
        }
    }
}