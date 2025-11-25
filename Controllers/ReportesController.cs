using Microsoft.AspNetCore.Mvc;
using WebApplication.Models.Dtos;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    private readonly IReportService _service;

    public ReportesController(IReportService service) => _service = service;

    [HttpGet("movimientos")]
    public async Task<ActionResult<IReadOnlyList<MovimientoItemDto>>> Movimientos(
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta,
        CancellationToken ct)
    {
        if (desde is null || hasta is null)
            return BadRequest("Debe enviar 'desde' y 'hasta' (ej: 2025-11-01).");

        var data = await _service.GetMovimientosAsync(desde.Value, hasta.Value, ct);
        return Ok(data);
    }

    [HttpGet("comparativo")]
    public async Task<ActionResult<IReadOnlyList<ComparativoItemDto>>> Comparativo(
        [FromQuery] DateTime desde,
        [FromQuery] DateTime hasta,
        [FromQuery] int? usuarioId,
        CancellationToken ct)
    {
        var data = await _service.GetComparativoAsync(desde, hasta, usuarioId, ct);
        return Ok(data);
    }
}
