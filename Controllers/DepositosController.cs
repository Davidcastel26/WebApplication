using Microsoft.AspNetCore.Mvc;
using WebApplication.Models.Dtos;
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
    public async Task<ActionResult<ApiResponse<DepositoDto>>> Create([FromBody] DepositoCreateDto dto, CancellationToken ct)
    {
        var id = await _service.CreateAsync(dto.Fecha, dto.FondoMonetarioId, dto.Monto, ct);
        var dep = await _service.GetByIdAsync(id, ct);
        if (dep is null)
            return Problem("Creado pero no se pudo leer el depósito.");

        var body = new DepositoDto(dep.Id, dep.Fecha, dep.FondoMonetarioId, dep.Monto);
        return Ok(new ApiResponse<DepositoDto>(200, "OK", body));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<DepositoDto>>> GetById(int id, CancellationToken ct)
    {
        var dep = await _service.GetByIdAsync(id, ct);
        if (dep is null)
            return NotFound(new ApiResponse<string>(404, "Not Found", $"Depósito {id} no existe"));

        var body = new DepositoDto(dep.Id, dep.Fecha, dep.FondoMonetarioId, dep.Monto);
        return Ok(new ApiResponse<DepositoDto>(200, "OK", body));
    }
}
