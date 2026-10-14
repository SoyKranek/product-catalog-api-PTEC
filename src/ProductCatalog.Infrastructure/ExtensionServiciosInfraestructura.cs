using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using ProductCatalog.Application.Productos;
using ProductCatalog.Infrastructure.Persistence;

namespace ProductCatalog.Infrastructure;

public static class ExtensionServiciosInfraestructura
{
    public static IServiceCollection AgregarInfraestructuraCatalogo(
        this IServiceCollection servicios,
        string cadenaConexion)
    {
        cadenaConexion = NormalizarCadenaConexion(cadenaConexion);
        servicios.AddSingleton(new InicializadorBaseDatos(cadenaConexion));
        servicios.AddSingleton<IRepositorioProducto>(new RepositorioProducto(cadenaConexion));
        return servicios;
    }

    private static string NormalizarCadenaConexion(string cadenaConexion)
    {
        if (!cadenaConexion.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
            && !cadenaConexion.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return cadenaConexion;
        }

        var uriBaseDatos = new Uri(cadenaConexion);
        var usuarioYClave = uriBaseDatos.UserInfo.Split(':', 2);
        var constructor = new NpgsqlConnectionStringBuilder
        {
            Host = uriBaseDatos.Host,
            Port = uriBaseDatos.Port > 0 ? uriBaseDatos.Port : 5432,
            Username = Uri.UnescapeDataString(usuarioYClave[0]),
            Password = usuarioYClave.Length > 1 ? Uri.UnescapeDataString(usuarioYClave[1]) : string.Empty,
            Database = uriBaseDatos.AbsolutePath.TrimStart('/')
        };

        if (uriBaseDatos.Query.Contains("sslmode=require", StringComparison.OrdinalIgnoreCase))
        {
            constructor.SslMode = SslMode.Require;
        }

        return constructor.ConnectionString;
    }
}
