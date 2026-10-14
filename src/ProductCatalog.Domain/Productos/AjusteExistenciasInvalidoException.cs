namespace ProductCatalog.Domain.Productos;

public sealed class AjusteExistenciasInvalidoException : Exception
{
    public AjusteExistenciasInvalidoException(string mensaje) : base(mensaje)
    {
    }
}
