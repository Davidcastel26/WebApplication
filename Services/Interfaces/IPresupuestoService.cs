using WebApplication.Models.Dtos;

namespace WebApplication.Services.Interfaces;

public interface IPresupuestoService
{
    /// Crea o actualiza un presupuesto (único por Anio, Mes, TipoGastoId, UsuarioId).
    Task<int> UpsertAsync(PresupuestoUpsertDto dto, CancellationToken ct = default);
}
