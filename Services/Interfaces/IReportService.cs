using WebApplication.Models.Dtos;

namespace WebApplication.Services.Interfaces;

public interface IReportService
{
    Task<IReadOnlyList<MovimientoItemDto>> GetMovimientosAsync(DateTime desde, DateTime hasta, CancellationToken ct = default);

    /// Comparativo de Presupuestado vs Ejecutado por TipoGasto en un rango de fechas.
    Task<IReadOnlyList<ComparativoItemDto>> GetComparativoAsync(DateTime desde, DateTime hasta, string? usuarioId, CancellationToken ct = default);
}
