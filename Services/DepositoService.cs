using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services;

public class DepositoService : IDepositoService
{
    private readonly ApplicationDbContext _dbContext;

    public DepositoService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> CreateAsync(DateTime transactionDate, int monetaryFundId, decimal amount, CancellationToken ct = default)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "El monto debe ser > 0.");

        var depositEntity = new Deposito
        {
            Fecha = transactionDate,
            FondoMonetarioId = monetaryFundId,
            Monto = amount
        };

        _dbContext.Depositos.Add(depositEntity);
        await _dbContext.SaveChangesAsync(ct);
        return depositEntity.Id;
    }

    public Task<Deposito?> GetByIdAsync(int depositId, CancellationToken ct = default) =>
        _dbContext.Depositos
                  .AsNoTracking()
                  .FirstOrDefaultAsync(d => d.Id == depositId, ct);
}
