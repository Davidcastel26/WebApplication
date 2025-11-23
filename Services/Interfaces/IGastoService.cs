using WebApplication.Models.Dtos;

namespace WebApplication.Services.Interfaces;

public interface IGastoService
{
    /// Crea un gasto (encabezado + detalle) en transacción.
    /// Valida presupuesto y retorna alertas de sobregiro (no bloquea el guardado por defecto).
    Task<GastoSaveResult> CreateAsync(GastoCreateRequest request, CancellationToken ct = default);
}
