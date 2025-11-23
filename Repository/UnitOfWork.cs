using Microsoft.EntityFrameworkCore.Storage;
using WebApplication.Repository.IRepository;

namespace WebApplication.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;

    public ITipoGastoRepository TipoGasto { get; }

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        TipoGasto = new TipoGastoRepository(_db);
        // Instancia los demás repos aquí cuando existan
    }

    public Task<int> SaveAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default) =>
        _db.Database.BeginTransactionAsync(ct);

    public ValueTask DisposeAsync() => _db.DisposeAsync();
}
