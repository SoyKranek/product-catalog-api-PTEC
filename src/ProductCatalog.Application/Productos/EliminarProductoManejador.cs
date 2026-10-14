namespace ProductCatalog.Application.Productos;

public sealed record EliminarProductoComando(Guid ProductoId);

public sealed class EliminarProductoManejador
{
    private readonly IRepositorioProducto _repositorioProducto;

    public EliminarProductoManejador(IRepositorioProducto repositorioProducto)
    {
        _repositorioProducto = repositorioProducto;
    }

    public async Task EjecutarAsync(EliminarProductoComando comando, CancellationToken cancelacion)
    {
        var eliminado = await _repositorioProducto.EliminarAsync(comando.ProductoId, cancelacion);
        if (!eliminado)
        {
            throw new ProductoNoEncontradoException(comando.ProductoId);
        }
    }
}
