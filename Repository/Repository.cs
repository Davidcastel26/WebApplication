using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApplication.Data;
using WebApplication.Repository.IRepository;

namespace WebApplication.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _db;
    protected readonly DbSet<T> _set;

    public Repository(ApplicationDbContext db)
    {
        _db = db;
        _set = _db.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _set.FindAsync(new object[] { id }, ct);   // 👈 evita el warning con la sobrecarga correcta

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        => await _set.AsNoTracking().ToListAsync(ct);

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _set.AsNoTracking().Where(predicate).ToListAsync(ct);

    public Task AddAsync(T entity, CancellationToken ct = default)
        => _set.AddAsync(entity, ct).AsTask();

    public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        => _set.AddRangeAsync(entities, ct);

    public void Update(T entity) => _set.Update(entity);
    public void Remove(T entity) => _set.Remove(entity);
    public void RemoveRange(IEnumerable<T> entities) => _set.RemoveRange(entities);

    public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => _set.AnyAsync(predicate, ct);
}
