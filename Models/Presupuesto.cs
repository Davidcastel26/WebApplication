public class Presupuesto
{
    public int Id { get; set; }
    public int Mes { get; set; }
    public int Anio { get; set; }
    public int TipoGastoId { get; set; }
    public TipoGasto TipoGasto { get; set; } = null!;
    public decimal MontoPresupuestado { get; set; }

    // Opción para multiusuario
    public string? UsuarioId { get; set; }
}
