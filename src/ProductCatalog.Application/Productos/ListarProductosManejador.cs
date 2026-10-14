namespace ProductCatalog.Application.Productos;

public sealed record ListarProductosConsulta(int NumeroPagina, int TamanoPagina);

public sealed class ListarProductosManejador
{
    private readonly IRepositorioProducto _repositorioProducto;

    public ListarProductosManejador(IRepositorioProducto repositorioProducto)
    {
        _repositorioProducto = repositorioProducto;
    }

    public async Task<ResultadoPaginado<ProductoRespuesta>> EjecutarAsync(ListarProductosConsulta consulta, CancellationToken cancelacion)
    {
        var pagina = await _repositorioProducto.ListarAsync(consulta.NumeroPagina, consulta.TamanoPagina, cancelacion);
        return new ResultadoPaginado<ProductoRespuesta>(
            pagina.Elementos.Select(MapeoProducto.ARespuesta).ToArray(),
            pagina.NumeroPagina,
            pagina.TamanoPagina,
            pagina.TotalRegistros);
    }
}
