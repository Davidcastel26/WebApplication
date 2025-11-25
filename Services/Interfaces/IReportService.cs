using WebApplication.Models.Dtos;
using WebApplication.Models; 
using WebApplication.Data;  
namespace WebApplication.Services.Interfaces;

public interface IReportService
{
    // Task<IReadOnlyList<MovimientoItemDto>> GetMovimientosAsync(DateTime desde, DateTime hasta, CancellationToken ct = default);

    /// Comparativo de Presupuestado vs Ejecutado por TipoGasto en un rango de fechas.
    // Task<IReadOnlyList<ComparativoItemDto>> GetComparativoAsync(DateTime desde, DateTime hasta, int? usuarioId, CancellationToken ct = default);
    Task<IReadOnlyList<MovimientoItemDto>> GetMovimientosAsync(DateTime desde, DateTime hasta, CancellationToken ct = default);
    Task<IReadOnlyList<ComparativoItemDto>> GetComparativoAsync(DateTime desde, DateTime hasta, int? usuarioId, CancellationToken ct = default);

}
