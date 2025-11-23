using Microsoft.EntityFrameworkCore;

using WebApplication.Models;
using WebApplication.Models.Dtos;
using WebApplication.Repository.IRepository;
using WebApplication.Services.Interfaces;

namespace WebApplication.Services;

public class GastoService : IGastoService
{
    private readonly ApplicationDbContext _db;
    private readonly IUnitOfWork _uow;

    public GastoService(ApplicationDbContext db, IUnitOfWork uow)
    {
        _db = db;
        _uow = uow;
    }

    public async Task<GastoSaveResult> CreateAsync(GastoCreateRequest request, CancellationToken ct = default)
    {
        if (request.Detalles is null || request.Detalles.Count == 0)
            throw new InvalidOperationException("No se puede guardar el gasto sin detalles.");

        // Mapear DTO -> entidad (encabezado)
        var encabezado = new GastoEncabezado
        {
            Fecha = request.Fecha,
            FondoMonetarioId = request.FondoMonetarioId,
            Observaciones = request.Observaciones ?? string.Empty,
            NombreComercio = request.NombreComercio,
            TipoDocumento = request.TipoDocumento switch
            {
                TipoDocumentoDto.Comprobante => TipoDocumento.Comprobante,
                TipoDocumentoDto.Factura => TipoDocumento.Factura,
                _ => TipoDocumento.Otro
            }
        };

        // Agregamos los detalles (map)
        encabezado.Detalles = request.Detalles
            .Select(d => new GastoDetalle
            {
                TipoGastoId = d.TipoGastoId,
                Monto = d.Monto
            })
            .ToList();

        // Calcular sobregiro antes de guardar (pero en la misma transacción)
        var anio = request.Fecha.Year;
        var mes = request.Fecha.Month;

        // Agrupamos lo nuevo por TipoGasto
        var nuevosPorTipo = encabezado.Detalles
            .GroupBy(d => d.TipoGastoId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Monto));

        var tipoGastoIds = nuevosPorTipo.Keys.ToList();

        // Ejecutado previo del mismo mes/año por tipo de gasto
        var ejecutadoPrevio = await _db.GastoDetalles
            .Where(d => tipoGastoIds.Contains(d.TipoGastoId)
                        && d.GastoEncabezado.Fecha.Year == anio
                        && d.GastoEncabezado.Fecha.Month == mes)
            .GroupBy(d => d.TipoGastoId)
            .Select(g => new { TipoGastoId = g.Key, Suma = g.Sum(x => x.Monto) })
            .ToListAsync(ct);

        var ejecutadoPrevioDict = ejecutadoPrevio.ToDictionary(x => x.TipoGastoId, x => x.Suma);

        // Presupuestos del mes por tipo de gasto (opcional por usuario)
        var presupuestos = await _db.Presupuestos
            .Where(p => p.Anio == anio && p.Mes == mes && tipoGastoIds.Contains(p.TipoGastoId)
                        && p.UsuarioId == request.UsuarioId)
            .Select(p => new { p.TipoGastoId, p.MontoPresupuestado })
            .ToListAsync(ct);

        var presupuestosDict = presupuestos.ToDictionary(x => x.TipoGastoId, x => x.MontoPresupuestado);

        // Obtener nombres de tipos de gasto para el reporte
        var tipoGastoNombres = await _db.TipoGastos
            .Where(t => tipoGastoIds.Contains(t.Id))
            .Select(t => new { t.Id, t.Nombre })
            .ToDictionaryAsync(t => t.Id, t => t.Nombre, ct);

        var sobregiros = new List<GastoOverdraftInfo>();

        foreach (var kvp in nuevosPorTipo)
        {
            var tipoId = kvp.Key;
            var montoNuevo = kvp.Value;
            var previo = ejecutadoPrevioDict.GetValueOrDefault(tipoId, 0m);
            var presup = presupuestosDict.GetValueOrDefault(tipoId, 0m); // si no hay presupuesto registrado, lo tomamos como 0

            var exceso = (previo + montoNuevo) - presup;
            if (exceso > 0)
            {
                sobregiros.Add(new GastoOverdraftInfo(
                    tipoId,
                    tipoGastoNombres.GetValueOrDefault(tipoId, $"TipoGasto {tipoId}"),
                    presup,
                    previo,
                    montoNuevo,
                    exceso
                ));
            }
        }

        // Transacción: guardar encabezado+detalles
        await using var trx = await _uow.BeginTransactionAsync(ct);
        try
        {
            _db.GastoEncabezados.Add(encabezado);
            await _uow.SaveAsync(ct);

            await trx.CommitAsync(ct);

            // Por requerimiento: "enviar alerta" => devolvemos la lista de sobregiros
            return new GastoSaveResult(encabezado.Id, Guardado: true, Sobregiros: sobregiros);
        }
        catch
        {
            await trx.RollbackAsync(ct);
            throw;
        }
    }
}
