using ECommerceAPI.Data;
using ECommerceAPI.Services;
using ECommerceAPI.Hubs; // ⭐ SignalR Hub

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using System.Text;
using System.Text.Json.Serialization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

////////////////////////////////////////////////////
// CONTROLLERS
////////////////////////////////////////////////////

builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

////////////////////////////////////////////////////
// DATABASE
////////////////////////////////////////////////////

builder.Services.AddDbContext<ECommerceDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new Exception("Database connection string missing")
    ));

////////////////////////////////////////////////////
// SERVICES
////////////////////////////////////////////////////

builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<InvoiceService>();

// ⭐ SignalR
builder.Services.AddSignalR();

// ⭐ Background worker for automatic order status update
builder.Services.AddHostedService<OrderStatusUpdater>();

////////////////////////////////////////////////////
// JWT AUTHENTICATION
////////////////////////////////////////////////////

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new Exception("JWT Key missing in appsettings.json");

builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        ),

        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

////////////////////////////////////////////////////
// CORS
////////////////////////////////////////////////////

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

////////////////////////////////////////////////////
// SWAGGER
////////////////////////////////////////////////////

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "ECommerce API",
            Version = "v1"
        });

    c.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header
        });

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
});

////////////////////////////////////////////////////
// BUILD APP
////////////////////////////////////////////////////

var app = builder.Build();

////////////////////////////////////////////////////
// MIDDLEWARE
////////////////////////////////////////////////////

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

////////////////////////////////////////////////////
// STATIC FILES (PRODUCT IMAGES)
////////////////////////////////////////////////////

app.UseStaticFiles();

////////////////////////////////////////////////////
// CORS
////////////////////////////////////////////////////

app.UseCors("AllowAngular");

////////////////////////////////////////////////////
// AUTHENTICATION
////////////////////////////////////////////////////

app.UseAuthentication();
app.UseAuthorization();

////////////////////////////////////////////////////
// MAP CONTROLLERS
////////////////////////////////////////////////////

app.MapControllers();

////////////////////////////////////////////////////
// ⭐ SIGNALR HUB
////////////////////////////////////////////////////

app.MapHub<OrderHub>("/orderHub");

////////////////////////////////////////////////////
// RUN APP
////////////////////////////////////////////////////

app.Run();