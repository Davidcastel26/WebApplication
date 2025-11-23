using Microsoft.AspNetCore.Mvc;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepositosController : ControllerBase
{
    private readonly IDepositoService _service;

    public DepositosController(IDepositoService service) => _service = service;

    public record DepositoCreateDto(DateTime Fecha, int FondoMonetarioId, decimal Monto);

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] DepositoCreateDto dto, CancellationToken ct)
    {
        var id = await _service.CreateAsync(dto.Fecha, dto.FondoMonetarioId, dto.Monto, ct);
        return CreatedAtAction(nameof(GetByIdPlaceholder), new { id }, id);
    }

    // Placeholder (si luego implementas GET por id)
    [HttpGet("{id:int}")]
    public IActionResult GetByIdPlaceholder(int id) => NoContent();
}
