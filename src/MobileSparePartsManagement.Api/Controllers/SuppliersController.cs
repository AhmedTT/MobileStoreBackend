using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileSparePartsManagement.Api.DTOs.Suppliers;
using MobileSparePartsManagement.Domain.Entities;
using MobileSparePartsManagement.Infrastructure.Data;

namespace MobileSparePartsManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly AppDbContext _context;

    public SuppliersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<SupplierDto>>> GetSuppliers([FromQuery] string? name)
    {
        var query = _context.Suppliers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(s => s.Name.Contains(name));
        }

        var suppliers = await query
            .Select(s => new SupplierDto
            {
                Id = s.Id,
                Name = s.Name,
                ContactEmail = s.ContactEmail,
                Phone = s.Phone,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();

        return Ok(suppliers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SupplierDto>> GetSupplier(Guid id)
    {
        var supplier = await _context.Suppliers
            .Where(s => s.Id == id)
            .Select(s => new SupplierDto
            {
                Id = s.Id,
                Name = s.Name,
                ContactEmail = s.ContactEmail,
                Phone = s.Phone,
                CreatedAt = s.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (supplier == null)
        {
            return NotFound(new { message = "Supplier not found" });
        }

        return Ok(supplier);
    }

    [HttpPost]
    public async Task<ActionResult<SupplierDto>> CreateSupplier([FromBody] CreateSupplierRequest request)
    {
        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ContactEmail = request.ContactEmail,
            Phone = request.Phone,
            CreatedAt = DateTime.UtcNow
        };

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        var supplierDto = new SupplierDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            ContactEmail = supplier.ContactEmail,
            Phone = supplier.Phone,
            CreatedAt = supplier.CreatedAt
        };

        return CreatedAtAction(nameof(GetSupplier), new { id = supplier.Id }, supplierDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SupplierDto>> UpdateSupplier(Guid id, [FromBody] UpdateSupplierRequest request)
    {
        var supplier = await _context.Suppliers.FindAsync(id);

        if (supplier == null)
        {
            return NotFound(new { message = "Supplier not found" });
        }

        supplier.Name = request.Name;
        supplier.ContactEmail = request.ContactEmail;
        supplier.Phone = request.Phone;

        await _context.SaveChangesAsync();

        var supplierDto = new SupplierDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            ContactEmail = supplier.ContactEmail,
            Phone = supplier.Phone,
            CreatedAt = supplier.CreatedAt
        };

        return Ok(supplierDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSupplier(Guid id)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.SpareParts)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (supplier == null)
        {
            return NotFound(new { message = "Supplier not found" });
        }

        if (supplier.SpareParts.Any())
        {
            return BadRequest(new { message = "Cannot delete supplier with existing spare parts" });
        }

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
