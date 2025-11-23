using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileSparePartsManagement.Api.DTOs.Auth;
using MobileSparePartsManagement.Domain.Entities;
using MobileSparePartsManagement.Infrastructure.Data;
using MobileSparePartsManagement.Infrastructure.Services;

namespace MobileSparePartsManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordService _passwordService;
    private readonly TokenService _tokenService;

    public AuthController(
        AppDbContext context,
        PasswordService passwordService,
        TokenService tokenService)
    {
        _context = context;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        try
        {
            if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // Password hash is invalid/corrupted - user needs to re-register
            return Unauthorized(new { message = "Invalid password format. Please contact administrator or re-register." });
        }

        var token = _tokenService.GenerateToken(user);

        var response = new LoginResponse
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email
            }
        };

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new { message = "Email already exists" });
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = _passwordService.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email
        };

        return CreatedAtAction(nameof(Register), new { id = user.Id }, userDto);
    }
}
