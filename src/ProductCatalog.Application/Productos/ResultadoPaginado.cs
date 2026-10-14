namespace ProductCatalog.Application.Productos;

public sealed record ResultadoPaginado<T>(
    IReadOnlyList<T> Elementos,
    int NumeroPagina,
    int TamanoPagina,
    int TotalRegistros);
