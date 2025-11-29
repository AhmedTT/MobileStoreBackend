using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileSparePartsManagement.Api.DTOs.Permissions;
using MobileSparePartsManagement.Domain.Entities;
using MobileSparePartsManagement.Infrastructure.Data;

namespace MobileSparePartsManagement.Api.Controllers;

[Authorize(Policy = "CanManageRoles")]
[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PermissionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<PermissionDto>>> GetPermissions()
    {
        var permissions = await _context.Permissions
            .Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            })
            .ToListAsync();

        return Ok(permissions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PermissionDto>> GetPermission(Guid id)
    {
        var permission = await _context.Permissions
            .Where(p => p.Id == id)
            .Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            })
            .FirstOrDefaultAsync();

        if (permission == null)
            return NotFound(new { message = "Permission not found" });

        return Ok(permission);
    }

    [HttpPost]
    public async Task<ActionResult<PermissionDto>> CreatePermission([FromBody] CreatePermissionRequest request)
    {
        if (await _context.Permissions.AnyAsync(p => p.Name == request.Name))
            return BadRequest(new { message = "Permission name already exists" });

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description
        };

        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();

        var permissionDto = new PermissionDto
        {
            Id = permission.Id,
            Name = permission.Name,
            Description = permission.Description
        };

        return CreatedAtAction(nameof(GetPermission), new { id = permission.Id }, permissionDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePermission(Guid id)
    {
        var permission = await _context.Permissions
            .Include(p => p.RolePermissions)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (permission == null)
            return NotFound(new { message = "Permission not found" });

        if (permission.RolePermissions.Any())
            return BadRequest(new { message = "Cannot delete permission assigned to roles" });

        _context.Permissions.Remove(permission);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}