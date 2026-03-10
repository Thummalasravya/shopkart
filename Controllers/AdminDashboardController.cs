using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Data;
using System.Linq;

namespace ECommerceAPI.Controllers
{

[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = "Admin")]

public class AdminDashboardController : ControllerBase
{

private readonly ECommerceDbContext _context;

public AdminDashboardController(ECommerceDbContext context)
{
_context = context;
}


//////////////////////////////////////
// GET DASHBOARD DATA
//////////////////////////////////////

[HttpGet]

public IActionResult GetDashboard()
{

//////////////////////////////////////
// COUNTS
//////////////////////////////////////

var totalUsers =
_context.Users.Count();


var totalOrders =
_context.Orders.Count();


var totalProducts =
_context.Products.Count();


var totalCategories =
_context.Categories.Count();



//////////////////////////////////////
// SAFE TOTAL REVENUE
//////////////////////////////////////

var totalRevenue =
_context.Orders.Any()
? _context.Orders.Sum(o => o.TotalAmount)
: 0;



//////////////////////////////////////
// MONTHLY REVENUE (GRAPH)
//////////////////////////////////////

var monthlyRevenue =

_context.Orders

.GroupBy(o => o.CreatedAt.Month)

.Select(g => new {

month = g.Key,

revenue = g.Sum(x => x.TotalAmount)

})

.OrderBy(x => x.month)

.ToList();



//////////////////////////////////////
// RECENT ORDERS
//////////////////////////////////////

var recentOrders =

_context.Orders

.OrderByDescending(o => o.CreatedAt)

.Take(5)

.Select(o => new
{
o.OrderId,
o.TotalAmount,
o.Status,
o.CreatedAt
})

.ToList();



//////////////////////////////////////
// RESPONSE
//////////////////////////////////////

return Ok(new {

totalUsers,
totalOrders,
totalProducts,
totalCategories,
totalRevenue,

monthlyRevenue,

recentOrders

});

}

}

}