namespace WebApplication.Models.Dtos;

public record GastoDetalleReadDto(
    int Id,
    int TipoGastoId,
    string TipoGastoNombre,
    decimal Monto
);

public record GastoReadDto(
    int Id,
    DateTime Fecha,
    int FondoMonetarioId,
    string? Observaciones,
    string? NombreComercio,
    string TipoDocumento,
    decimal Total,
    List<GastoDetalleReadDto> Detalles
);
