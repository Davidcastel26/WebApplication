using Microsoft.EntityFrameworkCore;
using WebApplication.Models.Dtos;
using WebApplication.Services.Interfaces;
using WebApplication.Models;
namespace WebApplication.Services;
using WebApplication.Data;  

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _db;

    public ReportService(ApplicationDbContext db) { _db = db; }

    public async Task<IReadOnlyList<MovimientoItemDto>> GetMovimientosAsync(
        DateTime desde,
        DateTime hasta,
        CancellationToken ct = default)
    {
        if (desde == default || hasta == default)
            throw new ArgumentException("Los parámetros 'desde' y 'hasta' son obligatorios.");
        if (desde > hasta)
            throw new ArgumentException("'desde' no puede ser mayor que 'hasta'.");

        var startDate = desde.Date;                 // 00:00:00 del día inicial
        var endDateExclusive = hasta.Date.AddDays(1); // 00:00:00 del día siguiente

        var gastos = await _db.GastoEncabezados
            .Where(e => e.Fecha >= startDate && e.Fecha < endDateExclusive)
            .Select(e => new MovimientoItemDto(
                MovimientoTipo.Gasto,
                e.Fecha,
                e.FondoMonetarioId,
                $"{e.NombreComercio} {(string.IsNullOrWhiteSpace(e.Observaciones) ? "" : $"- {e.Observaciones}")}",
                e.Detalles.Sum(d => d.Monto)
            ))
            .ToListAsync(ct);

        var depositos = await _db.Depositos
            .Where(d => d.Fecha >= startDate && d.Fecha < endDateExclusive)
            .Select(d => new MovimientoItemDto(
                MovimientoTipo.Deposito,
                d.Fecha,
                d.FondoMonetarioId,
                "Depósito",
                d.Monto
            ))
            .ToListAsync(ct);

        return gastos.Concat(depositos).OrderBy(m => m.Fecha).ToList();
    }

    public async Task<IReadOnlyList<ComparativoItemDto>> GetComparativoAsync(
    DateTime desde, DateTime hasta, int? usuarioId, CancellationToken ct = default)
{
    if (desde == default || hasta == default)
        throw new ArgumentException("Los parámetros 'desde' y 'hasta' son obligatorios.");
    if (desde > hasta)
        throw new ArgumentException("'desde' no puede ser mayor que 'hasta'.");

    // Ejecutado por TipoGasto en rango de fechas
    var ejecutado = await _db.GastoDetalles
        .Where(d => d.GastoEncabezado.Fecha >= desde && d.GastoEncabezado.Fecha <= hasta)
        .GroupBy(d => new { d.TipoGastoId, d.TipoGasto.Nombre })
        .Select(g => new
        {
            g.Key.TipoGastoId,
            TipoGastoNombre = g.Key.Nombre,
            Ejecutado = g.Sum(x => x.Monto)
        })
        .ToListAsync(ct);

    // Crear claves (AAMM) para el rango de meses
    var keys = EnumerarMeses(desde, hasta)
        .Select(m => m.Anio * 100 + m.Mes)
        .ToList();

    // si es null, traemos presupuestos con UsuarioId NULL
    var presupuestosQuery = _db.Presupuestos
        .Where(p => keys.Contains(p.Anio * 100 + p.Mes));

    if (usuarioId is null)
        presupuestosQuery = presupuestosQuery.Where(p => p.UsuarioId == null);
    else
        presupuestosQuery = presupuestosQuery.Where(p => p.UsuarioId == usuarioId);

    var presupuestos = await presupuestosQuery
        .GroupBy(p => new { p.TipoGastoId, p.TipoGasto.Nombre })
        .Select(g => new
        {
            g.Key.TipoGastoId,
            TipoGastoNombre = g.Key.Nombre,
            Presupuestado = g.Sum(x => x.MontoPresupuestado)
        })
        .ToListAsync(ct);

    // Unir ambos resultados
    var dict = new Dictionary<int, ComparativoItemDto>();

    foreach (var p in presupuestos)
    {
        dict[p.TipoGastoId] = new ComparativoItemDto(
            p.TipoGastoId, p.TipoGastoNombre, p.Presupuestado, 0m);
    }

    foreach (var e in ejecutado)
    {
        if (dict.TryGetValue(e.TipoGastoId, out var item))
        {
            dict[e.TipoGastoId] = item with { Ejecutado = e.Ejecutado };
        }
        else
        {
            dict[e.TipoGastoId] = new ComparativoItemDto(
                e.TipoGastoId, e.TipoGastoNombre, 0m, e.Ejecutado);
        }
    }

    return dict.Values.OrderBy(x => x.TipoGastoNombre).ToList();

    static IEnumerable<(int Anio, int Mes)> EnumerarMeses(DateTime desde, DateTime hasta)
    {
        var cursor = new DateTime(desde.Year, desde.Month, 1);
        var fin = new DateTime(hasta.Year, hasta.Month, 1);
        while (cursor <= fin)
        {
            yield return (cursor.Year, cursor.Month);
            cursor = cursor.AddMonths(1);
        }
    }
}

}
