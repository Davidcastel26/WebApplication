using Microsoft.EntityFrameworkCore;
using WebApplication.Models;
using WebApplication.Models.Dtos;
using WebApplication.Services.Interfaces;
using WebApplication.Data;  

namespace WebApplication.Services;

public class PresupuestoService : IPresupuestoService
{
    private readonly ApplicationDbContext _db;

    public PresupuestoService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<int> UpsertAsync(PresupuestoUpsertDto dto, CancellationToken ct = default)
    {
        var existing = await _db.Presupuestos
            .FirstOrDefaultAsync(p =>
                p.Anio == dto.Anio &&
                p.Mes == dto.Mes &&
                p.TipoGastoId == dto.TipoGastoId &&
                p.UsuarioId == dto.UsuarioId, ct);

        if (existing is null)
        {
            var nuevo = new Presupuesto
            {
                Anio = dto.Anio,
                Mes = dto.Mes,
                TipoGastoId = dto.TipoGastoId,
                MontoPresupuestado = dto.MontoPresupuestado,
                UsuarioId = dto.UsuarioId
            };
            _db.Presupuestos.Add(nuevo);
            await _db.SaveChangesAsync(ct);
            return nuevo.Id;
        }
        else
        {
            existing.MontoPresupuestado = dto.MontoPresupuestado;
            _db.Presupuestos.Update(existing);
            await _db.SaveChangesAsync(ct);
            return existing.Id;
        }
    }
}
