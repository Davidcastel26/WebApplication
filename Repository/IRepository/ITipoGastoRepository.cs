
namespace WebApplication.Repository.IRepository;
using WebApplication.Models;
public interface ITipoGastoRepository : IRepository<TipoGasto>
{
    /// Genera el siguiente código disponible. Formato por defecto: "TG-0001"
    Task<string> GenerateNextCodigoAsync(CancellationToken ct = default);

    Task<bool> CodigoExistsAsync(string codigo, CancellationToken ct = default);
}
