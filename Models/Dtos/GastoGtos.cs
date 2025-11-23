namespace WebApplication.Models.Dtos;

public enum TipoDocumentoDto
{
    Comprobante,
    Factura,
    Otro
}

public record GastoDetalleCreateDto(int TipoGastoId, decimal Monto);

public record GastoCreateRequest(
    DateTime Fecha,
    int FondoMonetarioId,
    string? Observaciones,
    string NombreComercio,
    TipoDocumentoDto TipoDocumento,
    List<GastoDetalleCreateDto> Detalles,
    string? UsuarioId // opcional para asociar contra Presupuesto
);

public record GastoOverdraftInfo(
    int TipoGastoId,
    string TipoGastoNombre,
    decimal Presupuestado,
    decimal EjecutadoPrevio,
    decimal MontoNuevo,
    decimal Exceso // (EjecutadoPrevio + MontoNuevo) - Presupuestado, si > 0
);

public record GastoSaveResult(
    int GastoEncabezadoId,
    bool Guardado,
    List<GastoOverdraftInfo> Sobregiros
);
