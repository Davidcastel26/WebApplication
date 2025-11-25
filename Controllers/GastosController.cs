using Microsoft.AspNetCore.Mvc;
using WebApplication.Models.Dtos;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GastosController : ControllerBase
{
    private readonly IGastoService _gastoService;

    public GastosController(IGastoService gastoService)
    {
        _gastoService = gastoService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<GastoSaveResult>>> Create(
        [FromBody] GastoCreateRequest gastoRequest,
        CancellationToken cancellationToken)
    {
        var saveResult = await _gastoService.CreateAsync(gastoRequest, cancellationToken);

        var response = new ApiResponse<GastoSaveResult>(201, "Created", saveResult);

        // 👉 Usa la acción real: nameof(GetById)
        return CreatedAtAction(
            actionName: nameof(GetById),
            routeValues: new { id = saveResult.GastoEncabezadoId },
            value: response
        );
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<GastoReadDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var gasto = await _gastoService.GetByIdAsync(id, cancellationToken);
        if (gasto is null)
        {
            return NotFound(new ApiResponse<string>(404, "Not Found", $"Gasto {id} no existe"));
        }

        var response = new ApiResponse<GastoReadDto>(200, "OK", gasto);
        return Ok(response);
    }
}
