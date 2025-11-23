using Microsoft.AspNetCore.Mvc;
using WebApplication.Models.Dtos;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PresupuestoController : ControllerBase
{
    private readonly IPresupuestoService _service;

    public PresupuestoController(IPresupuestoService service) => _service = service;

    [HttpPost("upsert")]
    public async Task<ActionResult<int>> Upsert([FromBody] PresupuestoUpsertDto dto, CancellationToken ct)
    {
        var id = await _service.UpsertAsync(dto, ct);
        return Ok(id);
    }
}
