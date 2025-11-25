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
    private readonly IPresupuestoService _service;
    private readonly ApplicationDbContext _db;

    public PresupuestoController(IPresupuestoService service, ApplicationDbContext db)
    {
        _service = service;
        _db = db;
    }

    [HttpPost("upsert")]
    public async Task<ActionResult<ApiResponse<PresupuestoDto>>> Upsert([FromBody] PresupuestoUpsertDto dto, CancellationToken ct)
    {
        var id = await _service.UpsertAsync(dto, ct);

        var entity = await _db.Presupuestos.AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PresupuestoDto(p.Id, p.Anio, p.Mes, p.TipoGastoId, p.MontoPresupuestado, p.UsuarioId))
            .FirstAsync(ct);

        return Ok(new ApiResponse<PresupuestoDto>(200, "OK", entity));
    }
}
