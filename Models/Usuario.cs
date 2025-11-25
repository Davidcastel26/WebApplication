namespace WebApplication.Models;

public class Usuario
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string Name { get; set; } = string.Empty;
    public string? Desc { get; set; }

    // Navegación opcional
    public ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();
}