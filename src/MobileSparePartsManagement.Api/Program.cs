using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MobileSparePartsManagement.Api.Authorization;
using MobileSparePartsManagement.Api.Middleware;
using MobileSparePartsManagement.Infrastructure.Data;
using MobileSparePartsManagement.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel for Railway
var portVar = builder.Configuration.GetValue<int?>("PORT");
if (portVar.HasValue && portVar.Value > 0)
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(portVar.Value);
    });
}

// Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Services
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<EmailService>();

// Authorization handlers
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
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

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Role-based policies
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireUser", policy => policy.RequireRole("User", "Admin"));

    // Permission-based policies
    options.AddPolicy("CanViewSpareParts", policy => 
        policy.Requirements.Add(new PermissionRequirement("ViewSpareParts")));
    
    options.AddPolicy("CanAddSpareParts", policy => 
        policy.Requirements.Add(new PermissionRequirement("AddSpareParts")));
    
    options.AddPolicy("CanEditSpareParts", policy => 
        policy.Requirements.Add(new PermissionRequirement("EditSpareParts")));
    
    options.AddPolicy("CanDeleteSpareParts", policy => 
        policy.Requirements.Add(new PermissionRequirement("DeleteSpareParts")));
    
    options.AddPolicy("CanManageSuppliers", policy => 
        policy.Requirements.Add(new PermissionRequirement("ManageSuppliers")));
    
    options.AddPolicy("CanManageRoles", policy => 
        policy.Requirements.Add(new PermissionRequirement("ManageRoles")));
});

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

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// Middleware pipeline
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();