using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Data;
using ECommerceAPI.Models;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public ReviewsController(ECommerceDbContext context)
        {
            _context = context;
        }

        // POST: api/Reviews
        [HttpPost]
        public async Task<IActionResult> AddReview(Review review)
        {
            if (review.Rating < 1 || review.Rating > 5)
            {
                return BadRequest("Rating must be between 1 and 5");
            }

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(review);
        }

        // GET: api/Reviews/product/1
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetReviewsByProduct(int productId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            return Ok(reviews);
        }
    }
}
