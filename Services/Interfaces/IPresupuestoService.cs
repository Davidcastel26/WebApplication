using WebApplication.Models.Dtos;
using WebApplication.Data;  
namespace WebApplication.Services.Interfaces;

public interface IPresupuestoService
{
    /// Crea o actualiza un presupuesto (único por Anio, Mes, TipoGastoId, UsuarioId).
    Task<int> UpsertAsync(PresupuestoUpsertDto dto, CancellationToken ct = default);
}
