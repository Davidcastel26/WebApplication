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
        [FromQuery] DateTime desde,
        [FromQuery] DateTime hasta,
        CancellationToken ct)
    {
        var data = await _service.GetMovimientosAsync(desde, hasta, ct);
        return Ok(data);
    }

    [HttpGet("comparativo")]
    public async Task<ActionResult<IReadOnlyList<ComparativoItemDto>>> Comparativo(
        [FromQuery] DateTime desde,
        [FromQuery] DateTime hasta,
        [FromQuery] string? usuarioId,
        CancellationToken ct)
    {
        var data = await _service.GetComparativoAsync(desde, hasta, usuarioId, ct);
        return Ok(data);
    }
}
