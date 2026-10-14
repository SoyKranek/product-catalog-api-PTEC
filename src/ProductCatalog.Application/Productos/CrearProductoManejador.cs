using ProductCatalog.Domain.Productos;

namespace ProductCatalog.Application.Productos;

public sealed record CrearProductoComando(string Nombre, string Descripcion, decimal PrecioUnitario, int ExistenciasIniciales);

public sealed class CrearProductoManejador
{
    private readonly IRepositorioProducto _repositorioProducto;

    public CrearProductoManejador(IRepositorioProducto repositorioProducto)
    {
        _repositorioProducto = repositorioProducto;
    }

    public async Task<ProductoRespuesta> EjecutarAsync(CrearProductoComando comando, CancellationToken cancelacion)
    {
        var producto = Producto.Crear(comando.Nombre, comando.Descripcion, comando.PrecioUnitario, comando.ExistenciasIniciales);
        await _repositorioProducto.AgregarAsync(producto, cancelacion);
        return MapeoProducto.ARespuesta(producto);
    }
}
