using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models.Dtos;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PresupuestoController : ControllerBase
{
    private readonly IPresupuestoService _presupuestoService;
    private readonly ApplicationDbContext _dbContext;

    public PresupuestoController(IPresupuestoService presupuestoService, ApplicationDbContext dbContext)
    {
        _presupuestoService = presupuestoService;
        _dbContext = dbContext;
    }
    
    [HttpPost("upsert")]
    public async Task<ActionResult<ApiResponse<PresupuestoDto>>> Upsert(
        [FromBody] PresupuestoUpsertDto upsertDto,
        CancellationToken cancellationToken)
    {
        var presupuestoId = await _presupuestoService.UpsertAsync(upsertDto, cancellationToken);

        var presupuestoDto = await _dbContext.Presupuestos
            .AsNoTracking()
            .Where(p => p.Id == presupuestoId)
            .Select(p => new PresupuestoDto(
                p.Id, p.Anio, p.Mes, p.TipoGastoId, p.MontoPresupuestado, p.UsuarioId))
            .FirstAsync(cancellationToken);

        return Ok(new ApiResponse<PresupuestoDto>(200, "OK", presupuestoDto));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<PresupuestoDto>>>> GetAll(
        [FromQuery] int? anio,
        [FromQuery] int? mes,
        [FromQuery] int? usuarioId,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Presupuestos
            .AsNoTracking()
            .AsQueryable();

        if (anio.HasValue)
            query = query.Where(p => p.Anio == anio.Value);

        if (mes.HasValue)
            query = query.Where(p => p.Mes == mes.Value);

        if (usuarioId.HasValue)
        query = query.Where(p => p.UsuarioId == usuarioId.Value);

        var items = await query
        .OrderBy(p => p.Anio).ThenBy(p => p.Mes).ThenBy(p => p.TipoGastoId)
        .Select(p => new PresupuestoDto(
            p.Id, p.Anio, p.Mes, p.TipoGastoId, p.MontoPresupuestado, p.UsuarioId))
        .ToListAsync(cancellationToken);

        return Ok(new ApiResponse<IEnumerable<PresupuestoDto>>(200, "OK", items));
    }

    [HttpGet("single")]
    public async Task<ActionResult<ApiResponse<PresupuestoDto?>>> GetSingle(
        [FromQuery] int anio,
        [FromQuery] int mes,
        [FromQuery] int tipoGastoId,
        [FromQuery] int? usuarioId,
        CancellationToken cancellationToken)
    {
        var item = await _dbContext.Presupuestos
            .AsNoTracking()
            .Where(p => p.Anio == anio
                        && p.Mes == mes
                        && p.TipoGastoId == tipoGastoId
                        && p.UsuarioId == usuarioId)
            .Select(p => new PresupuestoDto(
                p.Id, p.Anio, p.Mes, p.TipoGastoId, p.MontoPresupuestado, p.UsuarioId))
            .FirstOrDefaultAsync(cancellationToken);

        return Ok(new ApiResponse<PresupuestoDto?>(200, "OK", item));
    }
}
