using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Models.Dtos;

namespace WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FondoMonetarioController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public FondoMonetarioController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public record FondoMonetarioCreateDto(string Nombre, TipoFondo TipoFondo, string? NumeroCuenta, string? Descripcion);
    public record FondoMonetarioUpdateDto(string Nombre, TipoFondo TipoFondo, string? NumeroCuenta, string? Descripcion);
    public record FondoMonetarioReadDto(int Id, string Nombre, string TipoFondo, string? NumeroCuenta, string? Descripcion);

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<FondoMonetarioReadDto>>>> GetAll(CancellationToken ct)
    {
        var items = await _dbContext.FondoMonetarios
            .AsNoTracking()
            .OrderBy(f => f.Id)
            .Select(f => new FondoMonetarioReadDto(f.Id, f.Nombre, f.TipoFondo.ToString(), f.NumeroCuenta, f.Descripcion))
            .ToListAsync(ct);

        return Ok(new ApiResponse<IEnumerable<FondoMonetarioReadDto>>(200, "OK", items));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<FondoMonetarioReadDto>>> GetById(int id, CancellationToken ct)
    {
        var f = await _dbContext.FondoMonetarios
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (f is null)
            return NotFound(new ApiResponse<string>(404, "Not Found", $"FondoMonetario {id} no existe"));

        var dto = new FondoMonetarioReadDto(f.Id, f.Nombre, f.TipoFondo.ToString(), f.NumeroCuenta, f.Descripcion);
        return Ok(new ApiResponse<FondoMonetarioReadDto>(200, "OK", dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<FondoMonetarioReadDto>>> Create([FromBody] FondoMonetarioCreateDto request, CancellationToken ct)
    {
        var entity = new FondoMonetario
        {
            Nombre = request.Nombre,
            TipoFondo = request.TipoFondo,
            NumeroCuenta = request.NumeroCuenta,
            Descripcion = request.Descripcion
        };

        _dbContext.FondoMonetarios.Add(entity);
        await _dbContext.SaveChangesAsync(ct);

        var dto = new FondoMonetarioReadDto(entity.Id, entity.Nombre, entity.TipoFondo.ToString(), entity.NumeroCuenta, entity.Descripcion);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new ApiResponse<FondoMonetarioReadDto>(201, "Created", dto));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<FondoMonetarioReadDto>>> Update(int id, [FromBody] FondoMonetarioUpdateDto request, CancellationToken ct)
    {
        var entity = await _dbContext.FondoMonetarios.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null)
            return NotFound(new ApiResponse<string>(404, "Not Found", $"FondoMonetario {id} no existe"));

        entity.Nombre = request.Nombre;
        entity.TipoFondo = request.TipoFondo;
        entity.NumeroCuenta = request.NumeroCuenta;
        entity.Descripcion = request.Descripcion;

        await _dbContext.SaveChangesAsync(ct);

        var dto = new FondoMonetarioReadDto(entity.Id, entity.Nombre, entity.TipoFondo.ToString(), entity.NumeroCuenta, entity.Descripcion);
        return Ok(new ApiResponse<FondoMonetarioReadDto>(200, "OK", dto));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var entity = await _dbContext.FondoMonetarios.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null)
            return NotFound(new ApiResponse<string>(404, "Not Found", $"FondoMonetario {id} no existe"));

        _dbContext.FondoMonetarios.Remove(entity);
        await _dbContext.SaveChangesAsync(ct);
        return Ok(new ApiResponse<string>(200, "OK", $"FondoMonetario {id} eliminado"));
    }
}
