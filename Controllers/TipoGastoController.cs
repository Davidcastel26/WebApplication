using Microsoft.AspNetCore.Mvc;
using WebApplication.Models; // <- por TipoGasto
using WebApplication.Repository.IRepository;

namespace WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TipoGastoController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public TipoGastoController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TipoGasto>>> GetAll(CancellationToken ct)
        => Ok(await _uow.TipoGasto.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TipoGasto>> GetById(int id, CancellationToken ct)
    {
        var item = await _uow.TipoGasto.GetByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    public record CreateTipoGastoDto(string Nombre, string? Descripcion);

    [HttpPost]
    public async Task<ActionResult<TipoGasto>> Create([FromBody] CreateTipoGastoDto dto, CancellationToken ct)
    {
        var codigo = await _uow.TipoGasto.GenerateNextCodigoAsync(ct);
        var entity = new TipoGasto { Codigo = codigo, Nombre = dto.Nombre, Descripcion = dto.Descripcion };

        await _uow.TipoGasto.AddAsync(entity, ct);
        await _uow.SaveAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    public record UpdateTipoGastoDto(string Nombre, string? Descripcion);

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTipoGastoDto dto, CancellationToken ct)
    {
        var entity = await _uow.TipoGasto.GetByIdAsync(id, ct);
        if (entity is null) return NotFound();
        entity.Nombre = dto.Nombre;
        entity.Descripcion = dto.Descripcion;
        _uow.TipoGasto.Update(entity);
        await _uow.SaveAsync(ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var entity = await _uow.TipoGasto.GetByIdAsync(id, ct);
        if (entity is null) return NotFound();
        _uow.TipoGasto.Remove(entity);
        await _uow.SaveAsync(ct);
        return NoContent();
    }
}
