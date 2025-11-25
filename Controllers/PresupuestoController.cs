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

        var response = new ApiResponse<PresupuestoDto>(200, "OK", presupuestoDto);
        return Ok(response);
    }
}
