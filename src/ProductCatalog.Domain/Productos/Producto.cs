namespace ProductCatalog.Domain.Productos;

/// <summary>
/// Producto del catálogo. El stock solo cambia con <see cref="AjustarExistencias"/>.
/// </summary>
public sealed class Producto
{
    public Guid ProductoId { get; }
    public string Nombre { get; private set; }
    public string Descripcion { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public int CantidadExistencias { get; private set; }
    public DateTime FechaCreacionUtc { get; }
    public DateTime FechaActualizacionUtc { get; private set; }

    private Producto(
        Guid productoId,
        string nombre,
        string descripcion,
        decimal precioUnitario,
        int cantidadExistencias,
        DateTime fechaCreacionUtc,
        DateTime fechaActualizacionUtc)
    {
        ProductoId = productoId;
        Nombre = nombre;
        Descripcion = descripcion;
        PrecioUnitario = precioUnitario;
        CantidadExistencias = cantidadExistencias;
        FechaCreacionUtc = fechaCreacionUtc;
        FechaActualizacionUtc = fechaActualizacionUtc;
    }

    public static Producto Crear(string nombre, string descripcion, decimal precioUnitario, int existenciasIniciales)
    {
        ValidarNombre(nombre);
        ValidarDescripcion(descripcion);
        ValidarPrecioUnitario(precioUnitario);
        ValidarCantidadExistencias(existenciasIniciales);

        var fechaCreacionUtc = DateTime.UtcNow;
        return new Producto(
            Guid.NewGuid(),
            nombre.Trim(),
            descripcion.Trim(),
            precioUnitario,
            existenciasIniciales,
            fechaCreacionUtc,
            fechaCreacionUtc);
    }

    public static Producto Restaurar(
        Guid productoId,
        string nombre,
        string descripcion,
        decimal precioUnitario,
        int cantidadExistencias,
        DateTime fechaCreacionUtc,
        DateTime fechaActualizacionUtc)
    {
        return new Producto(
            productoId,
            nombre,
            descripcion,
            precioUnitario,
            cantidadExistencias,
            fechaCreacionUtc,
            fechaActualizacionUtc);
    }

    public void Renombrar(string nombre)
    {
        ValidarNombre(nombre);
        Nombre = nombre.Trim();
        FechaActualizacionUtc = DateTime.UtcNow;
    }

    public void CambiarDescripcion(string descripcion)
    {
        ValidarDescripcion(descripcion);
        Descripcion = descripcion.Trim();
        FechaActualizacionUtc = DateTime.UtcNow;
    }

    public void CambiarPrecioUnitario(decimal precioUnitario)
    {
        ValidarPrecioUnitario(precioUnitario);
        PrecioUnitario = precioUnitario;
        FechaActualizacionUtc = DateTime.UtcNow;
    }

    public void AjustarExistencias(int cambioCantidad)
    {
        if (cambioCantidad == 0)
        {
            throw new AjusteExistenciasInvalidoException("El cambio de cantidad debe ser distinto de cero.");
        }

        var existenciasResultantes = CantidadExistencias + cambioCantidad;
        if (existenciasResultantes < 0)
        {
            throw new ExistenciasInsuficientesException(CantidadExistencias, cambioCantidad);
        }

        CantidadExistencias = existenciasResultantes;
        FechaActualizacionUtc = DateTime.UtcNow;
    }

    private static void ValidarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre) || nombre.Trim().Length > 120)
        {
            throw new ArgumentException("El nombre es obligatorio y admite como máximo 120 caracteres.", nameof(nombre));
        }
    }

    private static void ValidarDescripcion(string descripcion)
    {
        if (descripcion is null || descripcion.Trim().Length > 1000)
        {
            throw new ArgumentException("La descripción admite como máximo 1000 caracteres.", nameof(descripcion));
        }
    }

    private static void ValidarPrecioUnitario(decimal precioUnitario)
    {
        if (precioUnitario < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(precioUnitario), "El precio unitario no puede ser negativo.");
        }

        if (decimal.Round(precioUnitario, 2, MidpointRounding.AwayFromZero) != precioUnitario)
        {
            throw new ArgumentException("El precio unitario admite como máximo dos decimales.", nameof(precioUnitario));
        }
    }

    private static void ValidarCantidadExistencias(int cantidadExistencias)
    {
        if (cantidadExistencias < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cantidadExistencias), "Las existencias no pueden ser negativas.");
        }
    }
}
