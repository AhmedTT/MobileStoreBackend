using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileSparePartsManagement.Api.DTOs.Common;
using MobileSparePartsManagement.Api.DTOs.SpareParts;
using MobileSparePartsManagement.Domain.Entities;
using MobileSparePartsManagement.Infrastructure.Data;

namespace MobileSparePartsManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SparePartsController : ControllerBase
{
    private readonly AppDbContext _context;

    public SparePartsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<SparePartDto>>> GetSpareParts(
        [FromQuery] string? name,
        [FromQuery] Guid? supplierId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string sortDir = "asc")
    {
        var query = _context.SpareParts
            .Include(sp => sp.Supplier)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(sp => sp.Name.Contains(name));
        }

        if (supplierId.HasValue)
        {
            query = query.Where(sp => sp.SupplierId == supplierId.Value);
        }

        // Apply sorting
        query = sortBy?.ToLower() switch
        {
            "name" => sortDir.ToLower() == "desc" 
                ? query.OrderByDescending(sp => sp.Name) 
                : query.OrderBy(sp => sp.Name),
            "price" => sortDir.ToLower() == "desc" 
                ? query.OrderByDescending(sp => sp.Price) 
                : query.OrderBy(sp => sp.Price),
            "quantity" => sortDir.ToLower() == "desc" 
                ? query.OrderByDescending(sp => sp.Quantity) 
                : query.OrderBy(sp => sp.Quantity),
            _ => query.OrderBy(sp => sp.CreatedAt)
        };

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination
        var spareParts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(sp => new SparePartDto
            {
                Id = sp.Id,
                Name = sp.Name,
                Quantity = sp.Quantity,
                Price = sp.Price,
                SupplierId = sp.SupplierId,
                SupplierName = sp.Supplier.Name,
                CreatedAt = sp.CreatedAt
            })
            .ToListAsync();

        var result = new PaginatedResult<SparePartDto>(spareParts, totalCount, page, pageSize);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SparePartDto>> GetSparePart(Guid id)
    {
        var sparePart = await _context.SpareParts
            .Include(sp => sp.Supplier)
            .Where(sp => sp.Id == id)
            .Select(sp => new SparePartDto
            {
                Id = sp.Id,
                Name = sp.Name,
                Quantity = sp.Quantity,
                Price = sp.Price,
                SupplierId = sp.SupplierId,
                SupplierName = sp.Supplier.Name,
                CreatedAt = sp.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (sparePart == null)
        {
            return NotFound(new { message = "Spare part not found" });
        }

        return Ok(sparePart);
    }

    [HttpPost]
    public async Task<ActionResult<SparePartDto>> CreateSparePart([FromBody] CreateSparePartRequest request)
    {
        // Validate spare part name doesn't already exist
        if (await _context.SpareParts.AnyAsync(sp => sp.Name == request.Name))
        {
            return BadRequest(new { message = "Spare part name already exists - ????? ?? ???" });
        }

        // Validate supplier exists
        if (!await _context.Suppliers.AnyAsync(s => s.Id == request.SupplierId))
        {
            return BadRequest(new { message = "Supplier not found" });
        }

        var sparePart = new SparePart
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Quantity = request.Quantity,
            Price = request.Price,
            SupplierId = request.SupplierId,
            CreatedAt = DateTime.UtcNow
        };

        _context.SpareParts.Add(sparePart);
        await _context.SaveChangesAsync();

        // Fetch with supplier info
        var createdSparePart = await _context.SpareParts
            .Include(sp => sp.Supplier)
            .Where(sp => sp.Id == sparePart.Id)
            .Select(sp => new SparePartDto
            {
                Id = sp.Id,
                Name = sp.Name,
                Quantity = sp.Quantity,
                Price = sp.Price,
                SupplierId = sp.SupplierId,
                SupplierName = sp.Supplier.Name,
                CreatedAt = sp.CreatedAt
            })
            .FirstAsync();

        return CreatedAtAction(nameof(GetSparePart), new { id = sparePart.Id }, createdSparePart);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SparePartDto>> UpdateSparePart(Guid id, [FromBody] UpdateSparePartRequest request)
    {
        var sparePart = await _context.SpareParts.FindAsync(id);

        if (sparePart == null)
        {
            return NotFound(new { message = "Spare part not found" });
        }

        // Validate supplier exists
        if (!await _context.Suppliers.AnyAsync(s => s.Id == request.SupplierId))
        {
            return BadRequest(new { message = "Supplier not found" });
        }

        sparePart.Name = request.Name;
        sparePart.Quantity = request.Quantity;
        sparePart.Price = request.Price;
        sparePart.SupplierId = request.SupplierId;

        await _context.SaveChangesAsync();

        // Fetch with supplier info
        var updatedSparePart = await _context.SpareParts
            .Include(sp => sp.Supplier)
            .Where(sp => sp.Id == id)
            .Select(sp => new SparePartDto
            {
                Id = sp.Id,
                Name = sp.Name,
                Quantity = sp.Quantity,
                Price = sp.Price,
                SupplierId = sp.SupplierId,
                SupplierName = sp.Supplier.Name,
                CreatedAt = sp.CreatedAt
            })
            .FirstAsync();

        return Ok(updatedSparePart);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSparePart(Guid id)
    {
        var sparePart = await _context.SpareParts.FindAsync(id);

        if (sparePart == null)
        {
            return NotFound(new { message = "Spare part not found" });
        }

        _context.SpareParts.Remove(sparePart);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
