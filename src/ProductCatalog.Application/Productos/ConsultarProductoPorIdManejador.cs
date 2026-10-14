namespace ProductCatalog.Application.Productos;

public sealed record ConsultarProductoPorIdConsulta(Guid ProductoId);

public sealed class ConsultarProductoPorIdManejador
{
    private readonly IRepositorioProducto _repositorioProducto;

    public ConsultarProductoPorIdManejador(IRepositorioProducto repositorioProducto)
    {
        _repositorioProducto = repositorioProducto;
    }

    public async Task<ProductoRespuesta> EjecutarAsync(ConsultarProductoPorIdConsulta consulta, CancellationToken cancelacion)
    {
        var producto = await _repositorioProducto.ObtenerPorIdAsync(consulta.ProductoId, cancelacion);
        if (producto is null)
        {
            throw new ProductoNoEncontradoException(consulta.ProductoId);
        }

        return MapeoProducto.ARespuesta(producto);
    }
}
