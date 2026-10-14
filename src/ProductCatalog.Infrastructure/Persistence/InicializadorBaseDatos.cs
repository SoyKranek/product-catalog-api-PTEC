using Npgsql;

namespace ProductCatalog.Infrastructure.Persistence;

public sealed class InicializadorBaseDatos
{
    private readonly string _cadenaConexion;

    public InicializadorBaseDatos(string cadenaConexion)
    {
        _cadenaConexion = cadenaConexion;
    }

    public async Task InicializarAsync(CancellationToken cancelacion)
    {
        var rutaScript = Path.Combine(AppContext.BaseDirectory, "Persistence", "create-products-table.sql");
        var script = await File.ReadAllTextAsync(rutaScript, cancelacion);

        await using var conexion = new NpgsqlConnection(_cadenaConexion);
        await conexion.OpenAsync(cancelacion);
        await using var comando = new NpgsqlCommand(script, conexion);
        await comando.ExecuteNonQueryAsync(cancelacion);
    }
}
