namespace ProductCatalog.Domain.Productos;

public sealed class ExistenciasInsuficientesException : Exception
{
    public ExistenciasInsuficientesException(int cantidadActual, int cambioCantidad)
        : base($"Las existencias no pueden quedar negativas. Hay {cantidadActual} y el cambio pedido es {cambioCantidad}.")
    {
        CantidadActual = cantidadActual;
        CambioCantidad = cambioCantidad;
    }

    public int CantidadActual { get; }
    public int CambioCantidad { get; }
}
