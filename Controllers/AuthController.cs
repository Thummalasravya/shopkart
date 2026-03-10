using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.DTOs;
using Google.Apis.Auth;

namespace ECommerceAPI.Controllers
{
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
private readonly IConfiguration _config;
private readonly ECommerceDbContext _context;

    public AuthController(
        IConfiguration config,
        ECommerceDbContext context)
    {
        _config = config;
        _context = context;
    }

    //////////////////////////////////////////////////////
    /// REGISTER
    //////////////////////////////////////////////////////

    [HttpPost("register")]
    public IActionResult Register(RegisterDto dto)
    {
        var exists =
            _context.Users.Any(u => u.Email == dto.Email);

        if (exists)
            return BadRequest(new
            {
                message = "User already exists"
            });

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            Provider = "Local",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        var role =
            _context.Roles
            .First(r => r.RoleName == "User");

        _context.UserRoles.Add(
            new UserRole
            {
                UserId = user.UserId,
                RoleId = role.Id
            });

        _context.SaveChanges();

        return Ok(new
        {
            message = "Registration successful"
        });
    }

    //////////////////////////////////////////////////////
    /// LOGIN (EMAIL + PASSWORD)
    //////////////////////////////////////////////////////

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {

        var user =
            _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefault(
                u => u.Email == dto.Email
            );

        if (user == null)
            return Unauthorized(new
            {
                message = "Invalid email"
            });

        var hashed =
            HashPassword(dto.Password);

        if (user.PasswordHash != hashed)
            return Unauthorized(new
            {
                message = "Invalid password"
            });

        var roles =
            user.UserRoles
            .Select(r => r.Role.RoleName)
            .ToList();

        var token =
            GenerateJwtToken(user, roles);

        return Ok(new
        {
            token = token,
            roles = roles,
            userId = user.UserId,
            name = user.Name
        });
    }

    //////////////////////////////////////////////////////
    /// GOOGLE LOGIN
    //////////////////////////////////////////////////////

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin(GoogleLoginDto dto)
    {
        try
        {
            var payload =
                await GoogleJsonWebSignature
                .ValidateAsync(dto.IdToken);

            var email = payload.Email;
            var name = payload.Name;
            var googleId = payload.Subject;

            var user =
                _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.Email == email);

            //////////////////////////////////////
            // CREATE USER IF NOT EXISTS
            //////////////////////////////////////

            if (user == null)
            {
                user = new User
                {
                    Name = name,
                    Email = email,
                    GoogleId = googleId,
                    Provider = "Google",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                var role =
                    _context.Roles
                    .First(r => r.RoleName == "User");

                _context.UserRoles.Add(
                    new UserRole
                    {
                        UserId = user.UserId,
                        RoleId = role.Id
                    });

                _context.SaveChanges();
            }

            //////////////////////////////////////
            // GET ROLES
            //////////////////////////////////////

            var roles =
                user.UserRoles
                .Select(r => r.Role.RoleName)
                .ToList();

            //////////////////////////////////////
            // GENERATE JWT
            //////////////////////////////////////

            var token =
                GenerateJwtToken(user, roles);

            return Ok(new
            {
                token = token,
                roles = roles,
                userId = user.UserId,
                name = user.Name
            });
        }
        catch
        {
            return Unauthorized(new
            {
                message = "Invalid Google token"
            });
        }
    }

    //////////////////////////////////////////////////////
    /// JWT TOKEN
    //////////////////////////////////////////////////////

    private string GenerateJwtToken(
        User user,
        List<string> roles)
    {

        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.UserId.ToString()
            ),

            new Claim(
                ClaimTypes.Email,
                user.Email
            )
        };

        foreach (var role in roles)
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Role,
                    role
                ));
        }

        var jwtKey =
            _config["Jwt:Key"]
            ?? "SuperSecretKey123456789";

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

        var creds =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

        var token =
            new JwtSecurityToken(

            issuer: "ECommerceAPI",

            audience: "ECommerceClient",

            claims: claims,

            expires: DateTime.UtcNow.AddDays(1),

            signingCredentials: creds
            );

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    //////////////////////////////////////////////////////
    /// PASSWORD HASH
    //////////////////////////////////////////////////////

    private string HashPassword(string password)
    {
        using var sha =
            SHA256.Create();

        var bytes =
            Encoding.UTF8.GetBytes(password);

        var hash =
            sha.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }

}

}
