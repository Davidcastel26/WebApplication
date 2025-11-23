using Microsoft.EntityFrameworkCore.Storage;

namespace WebApplication.Repository.IRepository;

public interface IUnitOfWork : IAsyncDisposable
{
    ITipoGastoRepository TipoGasto { get; }

    Task<int> SaveAsync(CancellationToken ct = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
}
