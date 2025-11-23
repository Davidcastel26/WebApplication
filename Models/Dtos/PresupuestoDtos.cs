namespace WebApplication.Models.Dtos;

public record PresupuestoUpsertDto(
    int Anio,
    int Mes,
    int TipoGastoId,
    decimal MontoPresupuestado,
    string? UsuarioId
);
