public enum TipoFondo
{
    CajaMenuda,
    CuentaBancaria
}

public class FondoMonetario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TipoFondo TipoFondo { get; set; }
    public string? NumeroCuenta { get; set; }
    public string? Descripcion { get; set; }

    public ICollection<GastoEncabezado> Gastos { get; set; } = new List<GastoEncabezado>();
    public ICollection<Deposito> Depositos { get; set; } = new List<Deposito>();
}
