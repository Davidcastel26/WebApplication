namespace WebApplication.Models.Dtos;

public enum MovimientoTipo
{
    Gasto,
    Deposito
}

public record MovimientoItemDto(
    MovimientoTipo Tipo,
    DateTime Fecha,
    int FondoMonetarioId,
    string? Descripcion, // gastos; "Depósito" para depósitos
    decimal MontoTotal
);

public record ComparativoItemDto(
    int TipoGastoId,
    string TipoGastoNombre,
    decimal Presupuestado,
    decimal Ejecutado
);
