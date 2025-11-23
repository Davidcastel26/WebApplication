
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services;

public class DepositoService : IDepositoService
{
    private readonly ApplicationDbContext _db;

    public DepositoService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<int> CreateAsync(DateTime fecha, int fondoMonetarioId, decimal monto, CancellationToken ct = default)
    {
        if (monto <= 0) throw new ArgumentOutOfRangeException(nameof(monto), "El monto debe ser > 0.");

        var dep = new Deposito
        {
            Fecha = fecha,
            FondoMonetarioId = fondoMonetarioId,
            Monto = monto
        };

        _db.Depositos.Add(dep);
        await _db.SaveChangesAsync(ct);
        return dep.Id;
    }
}
