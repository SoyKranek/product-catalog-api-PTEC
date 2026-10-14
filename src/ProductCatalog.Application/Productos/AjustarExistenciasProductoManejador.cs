using ProductCatalog.Domain.Productos;

namespace ProductCatalog.Application.Productos;

public sealed record AjustarExistenciasProductoComando(Guid ProductoId, int CambioCantidad);

public sealed class AjustarExistenciasProductoManejador
{
    private const int IntentosMaximos = 2;
    private readonly IRepositorioProducto _repositorioProducto;

    public AjustarExistenciasProductoManejador(IRepositorioProducto repositorioProducto)
    {
        _repositorioProducto = repositorioProducto;
    }

    public async Task<ProductoRespuesta> EjecutarAsync(AjustarExistenciasProductoComando comando, CancellationToken cancelacion)
    {
        if (comando.CambioCantidad == 0)
        {
            throw new AjusteExistenciasInvalidoException("El cambio de cantidad debe ser distinto de cero.");
        }

        for (var intento = 1; intento <= IntentosMaximos; intento++)
        {
            var productoAjustado = await _repositorioProducto.AjustarExistenciasAsync(
                comando.ProductoId,
                comando.CambioCantidad,
                cancelacion);

            if (productoAjustado is not null)
            {
                return MapeoProducto.ARespuesta(productoAjustado);
            }

            var productoActual = await _repositorioProducto.ObtenerPorIdAsync(comando.ProductoId, cancelacion);
            if (productoActual is null)
            {
                throw new ProductoNoEncontradoException(comando.ProductoId);
            }

            if (productoActual.CantidadExistencias + comando.CambioCantidad < 0)
            {
                throw new ExistenciasInsuficientesException(productoActual.CantidadExistencias, comando.CambioCantidad);
            }
        }

        throw new ConflictoExistenciasException(comando.ProductoId);
    }
}
