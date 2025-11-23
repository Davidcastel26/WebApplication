using Microsoft.AspNetCore.Mvc;
using WebApplication.Models.Dtos;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GastosController : ControllerBase
{
    private readonly IGastoService _service;

    public GastosController(IGastoService service) => _service = service;

    [HttpPost]
    public async Task<ActionResult<GastoSaveResult>> Create([FromBody] GastoCreateRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct);

        // Devuelve 201 con el Id del encabezado y lista de sobregiros (si hubiera)
        return CreatedAtAction(
            actionName: nameof(GetByIdPlaceholder),
            routeValues: new { id = result.GastoEncabezadoId },
            value: result
        );
    }

    // Placeholder (si luego implementas GET por id)
    [HttpGet("{id:int}")]
    public IActionResult GetByIdPlaceholder(int id) => NoContent();
}
