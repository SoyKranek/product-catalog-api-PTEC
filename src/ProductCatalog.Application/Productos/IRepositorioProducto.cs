using ProductCatalog.Domain.Productos;

namespace ProductCatalog.Application.Productos;

public interface IRepositorioProducto
{
    Task AgregarAsync(Producto producto, CancellationToken cancelacion);
    Task<Producto?> ObtenerPorIdAsync(Guid productoId, CancellationToken cancelacion);
    Task<ResultadoPaginado<Producto>> ListarAsync(int numeroPagina, int tamanoPagina, CancellationToken cancelacion);
    Task<bool> ActualizarAsync(Producto producto, CancellationToken cancelacion);
    Task<bool> EliminarAsync(Guid productoId, CancellationToken cancelacion);
    Task<Producto?> AjustarExistenciasAsync(Guid productoId, int cambioCantidad, CancellationToken cancelacion);
}
