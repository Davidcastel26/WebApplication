namespace WebApplication.Models.Dtos;

public record PresupuestoDto(
    int Id,
    int Anio,
    int Mes,
    int TipoGastoId,
    decimal MontoPresupuestado,
    int? UsuarioId
);

public record PresupuestoUpsertDto(
    int Anio,
    int Mes,
    int TipoGastoId,
    decimal MontoPresupuestado,
    int? UsuarioId 
);
