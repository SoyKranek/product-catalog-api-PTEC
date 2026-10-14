namespace ProductCatalog.Application.Productos;

public sealed record ProductoRespuesta(
    Guid ProductoId,
    string Nombre,
    string Descripcion,
    decimal PrecioUnitario,
    int CantidadExistencias,
    DateTime FechaCreacionUtc,
    DateTime FechaActualizacionUtc);
