using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MobileSparePartsManagement.Api.Middleware;
using MobileSparePartsManagement.Infrastructure.Data;
using MobileSparePartsManagement.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ==============================================================================
// 🛠️ FIX 1: Configure Kestrel to listen on the Railway provided PORT.
// This must be done on the builder.WebHost before builder.Build().
// ==============================================================================
var portVar = builder.Configuration.GetValue<int?>("PORT");

if (portVar.HasValue && portVar.Value > 0)
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(portVar.Value);
    });
}
// ==============================================================================

// Add services to the container.

// Database Context
// 🛠️ FIX 2: Use GetConnectionString("Default") which points to the DATABASE_URL 
// provided by Railway's Postgres service when linked. This is preferred 
// over hardcoding a proxy URL in appsettings.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Services
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<TokenService>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
// Use null-coalescing with a descriptive exception for clarity
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured.");
var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured.");
var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

// CORS
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:4200" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Controllers
builder.Services.AddControllers();

// OpenAPI (built-in .NET 9 support - assuming you are on .NET 8/9 preview)
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Global exception handling middleware (first)
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// OpenAPI (Development only)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// NOTE: UseHttpsRedirection relies on Kestrel knowing the port, which is now configured.
// It should be fine to keep, but can sometimes cause issues in complex proxy environments.
app.UseHttpsRedirection();

// ❌ REMOVED: The incorrect port configuration line is removed here.
// if(app.Environment.IsProduction()&& builder.Configuration.GetValue<int?>("PORT") is not null)
//     builder.WebHost.UseUrls($"http://*:{builder.Configuration.GetValue<int>("PORT")}");

// CORS
app.UseCors("AllowAngular");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

app.Run();