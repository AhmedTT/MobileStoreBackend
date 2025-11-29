using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileSparePartsManagement.Api.DTOs.Roles;
using MobileSparePartsManagement.Domain.Entities;
using MobileSparePartsManagement.Infrastructure.Data;

namespace MobileSparePartsManagement.Api.Controllers;

[Authorize(Policy = "CanManageRoles")]
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly AppDbContext _context;

    public RolesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<RoleDto>>> GetRoles()
    {
        var roles = await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Permissions = r.RolePermissions
                    .Select(rp => rp.Permission.Name)
                    .ToList()
            })
            .ToListAsync();

        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoleDto>> GetRole(Guid id)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Where(r => r.Id == id)
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Permissions = r.RolePermissions
                    .Select(rp => rp.Permission.Name)
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (role == null)
            return NotFound(new { message = "Role not found" });

        return Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleRequest request)
    {
        if (await _context.Roles.AnyAsync(r => r.Name == request.Name))
            return BadRequest(new { message = "Role name already exists" });

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        var roleDto = new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Permissions = new List<string>()
        };

        return CreatedAtAction(nameof(GetRole), new { id = role.Id }, roleDto);
    }

    [HttpPost("{id}/permissions")]
    public async Task<ActionResult> AssignPermissions(Guid id, [FromBody] AssignPermissionsRequest request)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (role == null)
            return NotFound(new { message = "Role not found" });

        // Validate all permissions exist
        var permissions = await _context.Permissions
            .Where(p => request.PermissionIds.Contains(p.Id))
            .ToListAsync();

        if (permissions.Count != request.PermissionIds.Count)
            return BadRequest(new { message = "One or more permissions not found" });

        // Remove existing permissions
        _context.RolePermissions.RemoveRange(role.RolePermissions);

        // Add new permissions
        foreach (var permissionId in request.PermissionIds)
        {
            role.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permissionId
            });
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Permissions assigned successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRole(Guid id)
    {
        var role = await _context.Roles
            .Include(r => r.Users)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (role == null)
            return NotFound(new { message = "Role not found" });

        if (role.Users.Any())
            return BadRequest(new { message = "Cannot delete role with assigned users" });

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}