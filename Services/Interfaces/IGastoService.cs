using WebApplication.Models.Dtos;

namespace WebApplication.Services.Interfaces;

public interface IGastoService
{
    Task<GastoSaveResult> CreateAsync(GastoCreateRequest request, CancellationToken ct = default);
    Task<GastoReadDto?> GetByIdAsync(int id, CancellationToken ct = default);
}
