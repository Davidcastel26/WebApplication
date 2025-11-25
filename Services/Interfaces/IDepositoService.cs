using WebApplication.Models;

namespace WebApplication.Services.Interfaces;

public interface IDepositoService
{
    Task<int> CreateAsync(DateTime transactionDate, int monetaryFundId, decimal amount, CancellationToken ct = default);
    Task<Deposito?> GetByIdAsync(int depositId, CancellationToken ct = default);
}
