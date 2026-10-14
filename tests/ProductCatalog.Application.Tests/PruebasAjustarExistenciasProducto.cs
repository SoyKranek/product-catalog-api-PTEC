using ProductCatalog.Application.Productos;
using ProductCatalog.Domain.Productos;

namespace ProductCatalog.Application.Tests;

public class PruebasAjustarExistenciasProducto
{
    [Fact]
    public async Task EjecutarAsync_CuandoElAjusteSeAplica_DevuelveProductoActualizado()
    {
        var productoId = Guid.NewGuid();
        var repositorio = new RepositorioProductoFalso
        {
            ProductoAjustado = Producto.Restaurar(productoId, "Café", "Bolsa", 10m, 7, DateTime.UtcNow, DateTime.UtcNow)
        };
        var manejador = new AjustarExistenciasProductoManejador(repositorio);

        var respuesta = await manejador.EjecutarAsync(new AjustarExistenciasProductoComando(productoId, 2), CancellationToken.None);

        Assert.Equal(7, respuesta.CantidadExistencias);
        Assert.Equal(productoId, respuesta.ProductoId);
    }

    [Fact]
    public async Task EjecutarAsync_CuandoElProductoNoExiste_LanzaProductoNoEncontradoException()
    {
        var manejador = new AjustarExistenciasProductoManejador(new RepositorioProductoFalso());

        await Assert.ThrowsAsync<ProductoNoEncontradoException>(() =>
            manejador.EjecutarAsync(new AjustarExistenciasProductoComando(Guid.NewGuid(), -1), CancellationToken.None));
    }

    [Fact]
    public async Task EjecutarAsync_CuandoLasExistenciasQuedarianNegativas_LanzaExistenciasInsuficientesException()
    {
        var productoId = Guid.NewGuid();
        var repositorio = new RepositorioProductoFalso
        {
            ProductoActual = Producto.Restaurar(productoId, "Café", "Bolsa", 10m, 1, DateTime.UtcNow, DateTime.UtcNow)
        };
        var manejador = new AjustarExistenciasProductoManejador(repositorio);

        await Assert.ThrowsAsync<ExistenciasInsuficientesException>(() =>
            manejador.EjecutarAsync(new AjustarExistenciasProductoComando(productoId, -5), CancellationToken.None));
    }

    [Fact]
    public async Task EjecutarAsync_CuandoElCambioEsCero_LanzaAjusteExistenciasInvalidoException()
    {
        var manejador = new AjustarExistenciasProductoManejador(new RepositorioProductoFalso());

        await Assert.ThrowsAsync<AjusteExistenciasInvalidoException>(() =>
            manejador.EjecutarAsync(new AjustarExistenciasProductoComando(Guid.NewGuid(), 0), CancellationToken.None));
    }

    private sealed class RepositorioProductoFalso : IRepositorioProducto
    {
        public Producto? ProductoAjustado { get; init; }
        public Producto? ProductoActual { get; init; }

        public Task AgregarAsync(Producto producto, CancellationToken cancelacion) => Task.CompletedTask;

        public Task<Producto?> ObtenerPorIdAsync(Guid productoId, CancellationToken cancelacion)
        {
            return Task.FromResult(ProductoActual);
        }

        public Task<ResultadoPaginado<Producto>> ListarAsync(int numeroPagina, int tamanoPagina, CancellationToken cancelacion)
        {
            return Task.FromResult(new ResultadoPaginado<Producto>(Array.Empty<Producto>(), numeroPagina, tamanoPagina, 0));
        }

        public Task<bool> ActualizarAsync(Producto producto, CancellationToken cancelacion) => Task.FromResult(true);

        public Task<bool> EliminarAsync(Guid productoId, CancellationToken cancelacion) => Task.FromResult(true);

        public Task<Producto?> AjustarExistenciasAsync(Guid productoId, int cambioCantidad, CancellationToken cancelacion)
        {
            return Task.FromResult(ProductoAjustado);
        }
    }
}
