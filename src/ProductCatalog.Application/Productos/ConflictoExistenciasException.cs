namespace ProductCatalog.Application.Productos;

public sealed class ConflictoExistenciasException : Exception
{
    public ConflictoExistenciasException(Guid productoId)
        : base($"Las existencias del producto {productoId} cambiaron mientras se aplicaba el ajuste. Reintente la operación.")
    {
        ProductoId = productoId;
    }

    public Guid ProductoId { get; }
}
