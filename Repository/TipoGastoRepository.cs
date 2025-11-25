using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Repository.IRepository;

namespace WebApplication.Repository;

public class TipoGastoRepository : Repository<TipoGasto>, ITipoGastoRepository
{
    private const string DefaultPrefix = "TG-";
    private const int PadLength = 4; // TG-0001

    public TipoGastoRepository(ApplicationDbContext db) : base(db) { }


    public async Task<bool> CodigoExistsAsync(string codigo, CancellationToken ct = default) =>
        await _set.AnyAsync(t => t.Codigo == codigo, ct);

    public async Task<string> GenerateNextCodigoAsync(CancellationToken ct = default)
    {
        // Busca los códigos existentes con el prefijo y obtiene el mayor sufijo numérico
        var codes = await _set
            .AsNoTracking()
            .Where(t => t.Codigo.StartsWith(DefaultPrefix))
            .Select(t => t.Codigo)
            .ToListAsync(ct);

        if (codes.Count == 0)
            return $"{DefaultPrefix}{1.ToString().PadLeft(PadLength, '0')}";

        var regex = new Regex(@"^" + Regex.Escape(DefaultPrefix) + @"(?<num>\d+)$");
        int max = 0;
        foreach (var code in codes)
        {
            var m = regex.Match(code);
            if (m.Success && int.TryParse(m.Groups["num"].Value, out var n))
                if (n > max) max = n;
        }

        var next = max + 1;
        return $"{DefaultPrefix}{next.ToString().PadLeft(PadLength, '0')}";
    }
}
