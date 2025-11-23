namespace WebApplication.Services.Interfaces;

public interface IDepositoService
{
    Task<int> CreateAsync(DateTime fecha, int fondoMonetarioId, decimal monto, CancellationToken ct = default);
}
