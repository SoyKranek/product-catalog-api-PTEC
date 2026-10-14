namespace ProductCatalog.Application.Productos;

public sealed class ProductoNoEncontradoException : Exception
{
    public ProductoNoEncontradoException(Guid productoId)
        : base($"No existe el producto {productoId}.")
    {
        ProductoId = productoId;
    }

    public Guid ProductoId { get; }
}
