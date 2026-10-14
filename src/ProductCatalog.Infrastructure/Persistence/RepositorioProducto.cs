using Dapper;
using Npgsql;
using ProductCatalog.Application.Productos;
using ProductCatalog.Domain.Productos;

namespace ProductCatalog.Infrastructure.Persistence;

public sealed class RepositorioProducto : IRepositorioProducto
{
    private readonly string _cadenaConexion;

    public RepositorioProducto(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    public async Task AgregarAsync(Producto producto, CancellationToken cancelacion)
    {
        const string sql = """
            INSERT INTO products (
                product_id, name, description, unit_price, stock_quantity, created_at_utc, updated_at_utc)
            VALUES (
                @ProductoId, @Nombre, @Descripcion, @PrecioUnitario, @CantidadExistencias, @FechaCreacionUtc, @FechaActualizacionUtc);
            """;

        await using var conexion = await AbrirConexionAsync(cancelacion);
        await conexion.ExecuteAsync(new CommandDefinition(sql, producto, cancellationToken: cancelacion));
    }

    public async Task<Producto?> ObtenerPorIdAsync(Guid productoId, CancellationToken cancelacion)
    {
        const string sql = """
            SELECT product_id AS ProductoId,
                   name AS Nombre,
                   description AS Descripcion,
                   unit_price AS PrecioUnitario,
                   stock_quantity AS CantidadExistencias,
                   created_at_utc AS FechaCreacionUtc,
                   updated_at_utc AS FechaActualizacionUtc
            FROM products
            WHERE product_id = @ProductoId;
            """;

        await using var conexion = await AbrirConexionAsync(cancelacion);
        var fila = await conexion.QuerySingleOrDefaultAsync<FilaProducto>(
            new CommandDefinition(sql, new { ProductoId = productoId }, cancellationToken: cancelacion));
        return fila?.AProducto();
    }

    public async Task<ResultadoPaginado<Producto>> ListarAsync(int numeroPagina, int tamanoPagina, CancellationToken cancelacion)
    {
        const string sqlConteo = "SELECT COUNT(*) FROM products;";
        const string sqlPagina = """
            SELECT product_id AS ProductoId,
                   name AS Nombre,
                   description AS Descripcion,
                   unit_price AS PrecioUnitario,
                   stock_quantity AS CantidadExistencias,
                   created_at_utc AS FechaCreacionUtc,
                   updated_at_utc AS FechaActualizacionUtc
            FROM products
            ORDER BY name ASC, product_id ASC
            LIMIT @TamanoPagina OFFSET @Desplazamiento;
            """;

        await using var conexion = await AbrirConexionAsync(cancelacion);
        var totalRegistros = await conexion.ExecuteScalarAsync<int>(new CommandDefinition(sqlConteo, cancellationToken: cancelacion));
        var filas = await conexion.QueryAsync<FilaProducto>(new CommandDefinition(
            sqlPagina,
            new { TamanoPagina = tamanoPagina, Desplazamiento = (numeroPagina - 1) * tamanoPagina },
            cancellationToken: cancelacion));

        return new ResultadoPaginado<Producto>(
            filas.Select(fila => fila.AProducto()).ToArray(),
            numeroPagina,
            tamanoPagina,
            totalRegistros);
    }

    public async Task<bool> ActualizarAsync(Producto producto, CancellationToken cancelacion)
    {
        const string sql = """
            UPDATE products
            SET name = @Nombre,
                description = @Descripcion,
                unit_price = @PrecioUnitario,
                updated_at_utc = @FechaActualizacionUtc
            WHERE product_id = @ProductoId;
            """;

        await using var conexion = await AbrirConexionAsync(cancelacion);
        var filasAfectadas = await conexion.ExecuteAsync(new CommandDefinition(sql, producto, cancellationToken: cancelacion));
        return filasAfectadas == 1;
    }

    public async Task<bool> EliminarAsync(Guid productoId, CancellationToken cancelacion)
    {
        const string sql = "DELETE FROM products WHERE product_id = @ProductoId;";
        await using var conexion = await AbrirConexionAsync(cancelacion);
        var filasAfectadas = await conexion.ExecuteAsync(
            new CommandDefinition(sql, new { ProductoId = productoId }, cancellationToken: cancelacion));
        return filasAfectadas == 1;
    }

    public async Task<Producto?> AjustarExistenciasAsync(Guid productoId, int cambioCantidad, CancellationToken cancelacion)
    {
        const string sql = """
            UPDATE products
            SET stock_quantity = stock_quantity + @CambioCantidad,
                updated_at_utc = @FechaActualizacionUtc
            WHERE product_id = @ProductoId
              AND stock_quantity + @CambioCantidad >= 0
            RETURNING product_id AS ProductoId,
                      name AS Nombre,
                      description AS Descripcion,
                      unit_price AS PrecioUnitario,
                      stock_quantity AS CantidadExistencias,
                      created_at_utc AS FechaCreacionUtc,
                      updated_at_utc AS FechaActualizacionUtc;
            """;

        await using var conexion = await AbrirConexionAsync(cancelacion);
        var fila = await conexion.QuerySingleOrDefaultAsync<FilaProducto>(new CommandDefinition(
            sql,
            new { ProductoId = productoId, CambioCantidad = cambioCantidad, FechaActualizacionUtc = DateTime.UtcNow },
            cancellationToken: cancelacion));
        return fila?.AProducto();
    }

    private async Task<NpgsqlConnection> AbrirConexionAsync(CancellationToken cancelacion)
    {
        var conexion = new NpgsqlConnection(_cadenaConexion);
        await conexion.OpenAsync(cancelacion);
        return conexion;
    }

    private sealed class FilaProducto
    {
        public Guid ProductoId { get; init; }
        public string Nombre { get; init; } = string.Empty;
        public string Descripcion { get; init; } = string.Empty;
        public decimal PrecioUnitario { get; init; }
        public int CantidadExistencias { get; init; }
        public DateTime FechaCreacionUtc { get; init; }
        public DateTime FechaActualizacionUtc { get; init; }

        public Producto AProducto()
        {
            return Producto.Restaurar(
                ProductoId,
                Nombre,
                Descripcion,
                PrecioUnitario,
                CantidadExistencias,
                FechaCreacionUtc,
                FechaActualizacionUtc);
        }
    }
}
