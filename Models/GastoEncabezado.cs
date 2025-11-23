public enum TipoDocumento
{
    Comprobante,
    Factura,
    Otro
}

public class GastoEncabezado
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int FondoMonetarioId { get; set; }
    public FondoMonetario FondoMonetario { get; set; } = null!;
    public string Observaciones { get; set; } = string.Empty;
    public string NombreComercio { get; set; } = string.Empty;
    public TipoDocumento TipoDocumento { get; set; }

    public ICollection<GastoDetalle> Detalles { get; set; } = new List<GastoDetalle>();
}
