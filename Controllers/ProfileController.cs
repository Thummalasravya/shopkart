using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;

namespace ECommerceAPI.Controllers
{

[ApiController]

[Route("api/profile")]

[Authorize]

public class ProfileController : ControllerBase
{

private readonly ECommerceDbContext _context;

public ProfileController(ECommerceDbContext context)
{
_context = context;
}


////////////////////////////////////////////////
/// GET PROFILE
////////////////////////////////////////////////

[HttpGet]

public IActionResult GetProfile()
{

// ⭐ Get Logged-in UserId from JWT

var userIdClaim =
User.FindFirst(ClaimTypes.NameIdentifier);

if(userIdClaim==null)
return Unauthorized();

int userId=
int.Parse(userIdClaim.Value);


// ⭐ Get User

var user=_context.Users
.FirstOrDefault(x=>x.UserId==userId);


if(user==null)
return NotFound();


// ⭐ Return Profile Data

return Ok(new{

name=user.Name,

email=user.Email

});

}



////////////////////////////////////////////////
/// UPDATE PROFILE
////////////////////////////////////////////////

[HttpPut]

public IActionResult UpdateProfile(
[FromBody] ProfileDto dto)
{


var userIdClaim =
User.FindFirst(ClaimTypes.NameIdentifier);

if(userIdClaim==null)
return Unauthorized();


int userId=
int.Parse(userIdClaim.Value);


// ⭐ Find User

var user=_context.Users
.FirstOrDefault(x=>x.UserId==userId);


if(user==null)
return NotFound();


// ⭐ Update Basic Info

user.Name=dto.Name;

user.Email=dto.Email;


// ⭐ Update Password (optional)

if(!string.IsNullOrWhiteSpace(dto.Password))
{

user.PasswordHash=
HashPassword(dto.Password);

}


// ⭐ Save

_context.SaveChanges();


// ⭐ Return Success

return Ok(new{

message="Profile Updated Successfully"

});

}



//////////////////////////////////////////
// PASSWORD HASH
//////////////////////////////////////////

private string HashPassword(string password)
{

using var sha=SHA256.Create();

var bytes=
Encoding.UTF8.GetBytes(password);

var hash=
sha.ComputeHash(bytes);

return Convert.ToBase64String(hash);

}


}

}