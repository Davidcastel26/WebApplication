namespace WebApplication.Models;

public class TipoGasto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public ICollection<GastoDetalle> GastoDetalles { get; set; } = new List<GastoDetalle>();
    public ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();
}
