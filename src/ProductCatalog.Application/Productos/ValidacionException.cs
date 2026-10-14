namespace ProductCatalog.Application.Productos;

public sealed class ValidacionException : Exception
{
    public ValidacionException(IReadOnlyList<string> errores)
        : base(errores.Count == 0 ? "La solicitud no es válida." : string.Join(" ", errores))
    {
        Errores = errores;
    }

    public IReadOnlyList<string> Errores { get; }
}
