namespace WebApplication.Models.Dtos;

public record DepositoDto(
    int Id,
    DateTime Fecha,
    int FondoMonetarioId,
    decimal Monto
);
