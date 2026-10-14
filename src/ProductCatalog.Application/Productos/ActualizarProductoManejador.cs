namespace ProductCatalog.Application.Productos;

public sealed record ActualizarProductoComando(Guid ProductoId, string Nombre, string Descripcion, decimal PrecioUnitario);

public sealed class ActualizarProductoManejador
{
    private readonly IRepositorioProducto _repositorioProducto;

    public ActualizarProductoManejador(IRepositorioProducto repositorioProducto)
    {
        _repositorioProducto = repositorioProducto;
    }

    public async Task<ProductoRespuesta> EjecutarAsync(ActualizarProductoComando comando, CancellationToken cancelacion)
    {
        var producto = await _repositorioProducto.ObtenerPorIdAsync(comando.ProductoId, cancelacion);
        if (producto is null)
        {
            throw new ProductoNoEncontradoException(comando.ProductoId);
        }

        producto.Renombrar(comando.Nombre);
        producto.CambiarDescripcion(comando.Descripcion);
        producto.CambiarPrecioUnitario(comando.PrecioUnitario);

        var actualizado = await _repositorioProducto.ActualizarAsync(producto, cancelacion);
        if (!actualizado)
        {
            throw new ProductoNoEncontradoException(comando.ProductoId);
        }

        return MapeoProducto.ARespuesta(producto);
    }
}
