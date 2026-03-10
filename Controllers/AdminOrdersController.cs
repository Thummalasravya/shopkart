using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Data;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/admin/orders")]
    [Authorize(Roles = "Admin,Manager")]
    public class AdminOrdersController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public AdminOrdersController(ECommerceDbContext context)
        {
            _context = context;
        }

        ////////////////////////////////////////////
        // GET ALL ORDERS
        ////////////////////////////////////////////

        [HttpGet]
        public IActionResult GetOrders()
        {
            var orders = _context.Orders
                .Include(o => o.Address)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return Ok(orders);
        }



        ////////////////////////////////////////////
        // GET ORDER BY ID
        ////////////////////////////////////////////

        [HttpGet("{id}")]
        public IActionResult GetOrder(int id)
        {
            var order = _context.Orders
                .Include(o => o.Address)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
                return NotFound("Order not found");

            return Ok(order);
        }



        ////////////////////////////////////////////
        // UPDATE ORDER STATUS
        ////////////////////////////////////////////

        [HttpPut("{id}/status")]
        public IActionResult UpdateStatus(
            int id,
            [FromBody] string status
        )
        {
            var validStatuses = new[]
            {
                "Placed",
                "Processing",
                "Shipped",
                "Delivered",
                "Cancelled"
            };

            // Validate status
            if (!validStatuses.Contains(status))
                return BadRequest("Invalid status");

            var order = _context.Orders.Find(id);

            if (order == null)
                return NotFound("Order not found");

            order.Status = status;

            _context.SaveChanges();

            return Ok(new
            {
                orderId = order.OrderId,
                status = order.Status,
                message = "Status updated successfully"
            });
        }

    }
}