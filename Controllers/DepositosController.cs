using Microsoft.AspNetCore.Mvc;
using WebApplication.Models.Dtos;
using WebApplication.Services.Interfaces;

namespace WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepositosController : ControllerBase
{
    private readonly IDepositoService _depositService;

    public DepositosController(IDepositoService depositService)
    {
        _depositService = depositService;
    }

    public record DepositoCreateDto(DateTime Fecha, int FondoMonetarioId, decimal Monto);

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DepositoDto>>> Create(
        [FromBody] DepositoCreateDto createRequest,
        CancellationToken cancellationToken)
    {
        var createdDepositId = await _depositService.CreateAsync(
            transactionDate: createRequest.Fecha,
            monetaryFundId: createRequest.FondoMonetarioId,
            amount: createRequest.Monto,
            ct: cancellationToken);

        var createdDeposit = await _depositService.GetByIdAsync(createdDepositId, cancellationToken);
        if (createdDeposit is null)
            return Problem("El depósito fue creado pero no se pudo recuperar.");

        var responseBody = new DepositoDto(
            createdDeposit.Id,
            createdDeposit.Fecha,
            createdDeposit.FondoMonetarioId,
            createdDeposit.Monto);

        return Ok(new ApiResponse<DepositoDto>(200, "OK", responseBody));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<DepositoDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var deposit = await _depositService.GetByIdAsync(id, cancellationToken);
        if (deposit is null)
            return NotFound(new ApiResponse<string>(404, "Not Found", $"Depósito {id} no existe"));

        var responseBody = new DepositoDto(deposit.Id, deposit.Fecha, deposit.FondoMonetarioId, deposit.Monto);
        return Ok(new ApiResponse<DepositoDto>(200, "OK", responseBody));
    }
}
