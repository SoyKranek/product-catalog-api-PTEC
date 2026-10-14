using ProductCatalog.Domain.Productos;

namespace ProductCatalog.Application.Productos;

public static class MapeoProducto
{
    public static ProductoRespuesta ARespuesta(Producto producto)
    {
        return new ProductoRespuesta(
            producto.ProductoId,
            producto.Nombre,
            producto.Descripcion,
            producto.PrecioUnitario,
            producto.CantidadExistencias,
            producto.FechaCreacionUtc,
            producto.FechaActualizacionUtc);
    }
}
