using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.Services;

namespace ECommerceAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ECommerceDbContext _context;
        private readonly EmailService _emailService;

        public OrdersController(ECommerceDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        ///////////////////////////////////////////////////
        // GET USER ID FROM JWT
        ///////////////////////////////////////////////////

        private int GetUserId()
        {
            var claim = User.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type == "nameid" ||
                c.Type == "sub");

            if (claim == null)
                throw new Exception("User ID missing in token");

            return int.Parse(claim.Value);
        }

        ///////////////////////////////////////////////////
        // PLACE ORDER
        ///////////////////////////////////////////////////

        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderRequest request)
        {
            int userId = GetUserId();

            if (request?.AddressId == null)
                return BadRequest("Address required");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return BadRequest("User not found");

            var cartItems = await _context.Carts
                .Where(c => c.UserId == userId && c.IsSelected)
                .Include(c => c.Product)
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest("Cart is empty");

            var order = new Order
            {
                UserId = userId,
                AddressId = request.AddressId,
                CreatedAt = DateTime.UtcNow,
                Status = "Placed",
                Items = new List<OrderItem>()
            };

            decimal total = 0;

            foreach (var cart in cartItems)
            {
                if (cart.Product == null) continue;

                decimal subtotal = cart.Product.Price * cart.Quantity;
                total += subtotal;

                order.Items.Add(new OrderItem
                {
                    ProductId = cart.Product.ProductId,
                    Quantity = cart.Quantity,
                    Price = cart.Product.Price
                });
            }

            order.TotalAmount = total;

            _context.Orders.Add(order);
            _context.Carts.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            ///////////////////////////////////////////////////
            // SEND EMAIL
            ///////////////////////////////////////////////////

            try
            {
                if (!string.IsNullOrEmpty(user.Email))
                {
                    string body = _emailService.BuildOrderEmailTemplate(order);

                    _emailService.SendEmail(
                        user.Email,
                        $"Order Confirmation #{order.OrderId}",
                        body
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email error: " + ex.Message);
            }

            return Ok(new
            {
                orderId = order.OrderId,
                total = order.TotalAmount
            });
        }

        ///////////////////////////////////////////////////
        // GET MY ORDERS
        ///////////////////////////////////////////////////

        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrders()
        {
            int userId = GetUserId();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new
                {
                    o.OrderId,
                    o.TotalAmount,
                    o.Status,
                    o.CreatedAt,

                    items = o.Items.Select(i => new
                    {
                        productId = i.ProductId,
                        quantity = i.Quantity,
                        price = i.Price,
                        productName = i.Product != null ? i.Product.Name : "Product",
                        image = i.Product != null
                            ? baseUrl + "/" + i.Product.ImageUrl
                            : ""
                    })
                })
                .ToListAsync();

            return Ok(orders);
        }

        ///////////////////////////////////////////////////
        // ORDER DETAILS
        ///////////////////////////////////////////////////

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            int userId = GetUserId();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var order = await _context.Orders
                .Where(o => o.OrderId == id && o.UserId == userId)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Select(o => new
                {
                    o.OrderId,
                    o.TotalAmount,
                    o.Status,
                    o.CreatedAt,

                    items = o.Items.Select(i => new
                    {
                        productId = i.ProductId,
                        quantity = i.Quantity,
                        price = i.Price,
                        productName = i.Product != null ? i.Product.Name : "Product",
                        image = i.Product != null
                            ? baseUrl + "/" + i.Product.ImageUrl
                            : ""
                    })
                })
                .FirstOrDefaultAsync();

            if (order == null)
                return NotFound("Order not found");

            return Ok(order);
        }

        ///////////////////////////////////////////////////

        public class OrderRequest
        {
            public int? AddressId { get; set; }
        }
    }
}