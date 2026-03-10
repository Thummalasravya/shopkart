using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔐 JWT PROTECTION
    public class SecureController : ControllerBase
    {
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);

            return Ok(new
            {
                message = "JWT authentication successful",
                email = email,
                role = role
            });
        }

        [HttpGet("orders")]
        public IActionResult GetOrders()
        {
            return Ok(new[]
            {
                new { OrderId = 1001, Total = 1999 },
                new { OrderId = 1002, Total = 2999 }
            });
        }
    }
}
