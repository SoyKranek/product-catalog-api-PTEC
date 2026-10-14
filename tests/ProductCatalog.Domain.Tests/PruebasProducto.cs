using ProductCatalog.Domain.Productos;

namespace ProductCatalog.Domain.Tests;

public class PruebasProducto
{
    [Fact]
    public void Crear_ConDatosValidos_DevuelveProducto()
    {
        var producto = Producto.Crear("Café molido", "Bolsa de 500 gramos", 18500.50m, 10);

        Assert.Equal("Café molido", producto.Nombre);
        Assert.Equal("Bolsa de 500 gramos", producto.Descripcion);
        Assert.Equal(18500.50m, producto.PrecioUnitario);
        Assert.Equal(10, producto.CantidadExistencias);
        Assert.NotEqual(Guid.Empty, producto.ProductoId);
    }

    [Fact]
    public void Crear_ConPrecioNegativo_LanzaArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Producto.Crear("Café", "Bolsa", -1m, 1));
    }

    [Fact]
    public void Crear_ConExistenciasInicialesNegativas_LanzaArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Producto.Crear("Café", "Bolsa", 10m, -1));
    }

    [Fact]
    public void AjustarExistencias_CuandoSuma_ActualizaCantidad()
    {
        var producto = Producto.Crear("Café", "Bolsa", 10m, 4);

        producto.AjustarExistencias(3);

        Assert.Equal(7, producto.CantidadExistencias);
    }

    [Fact]
    public void AjustarExistencias_CuandoLlegaACero_ActualizaCantidad()
    {
        var producto = Producto.Crear("Café", "Bolsa", 10m, 4);

        producto.AjustarExistencias(-4);

        Assert.Equal(0, producto.CantidadExistencias);
    }

    [Fact]
    public void AjustarExistencias_CuandoQuedariaNegativo_LanzaExistenciasInsuficientesException()
    {
        var producto = Producto.Crear("Café", "Bolsa", 10m, 2);

        Assert.Throws<ExistenciasInsuficientesException>(() => producto.AjustarExistencias(-3));
        Assert.Equal(2, producto.CantidadExistencias);
    }

    [Fact]
    public void AjustarExistencias_CuandoCambioEsCero_LanzaAjusteExistenciasInvalidoException()
    {
        var producto = Producto.Crear("Café", "Bolsa", 10m, 2);

        Assert.Throws<AjusteExistenciasInvalidoException>(() => producto.AjustarExistencias(0));
    }
}
