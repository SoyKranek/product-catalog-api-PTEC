using FluentValidation;
using Microsoft.OpenApi.Models;
using ProductCatalog.Api.Errores;
using ProductCatalog.Application.Productos;
using ProductCatalog.Infrastructure;
using ProductCatalog.Infrastructure.Persistence;
using Serilog;

var constructor = WebApplication.CreateBuilder(args);

constructor.Host.UseSerilog((_, configuracion) =>
    configuracion.WriteTo.Console().MinimumLevel.Information());

var cadenaConexion = constructor.Configuration.GetConnectionString("ProductCatalog")
    ?? throw new InvalidOperationException("La cadena de conexión 'ProductCatalog' es obligatoria.");

constructor.Services.AgregarInfraestructuraCatalogo(cadenaConexion);
constructor.Services.AddScoped<CrearProductoManejador>();
constructor.Services.AddScoped<ConsultarProductoPorIdManejador>();
constructor.Services.AddScoped<ListarProductosManejador>();
constructor.Services.AddScoped<ActualizarProductoManejador>();
constructor.Services.AddScoped<EliminarProductoManejador>();
constructor.Services.AddScoped<AjustarExistenciasProductoManejador>();
constructor.Services.AddValidatorsFromAssemblyContaining<Program>();
constructor.Services.AddExceptionHandler<ManejadorExcepcionesCatalogo>();
constructor.Services.AddProblemDetails();
constructor.Services.AddControllers();
constructor.Services.AddEndpointsApiExplorer();
constructor.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Catálogo de productos",
        Version = "v1",
        Description = "API para administrar el catálogo de productos y sus existencias."
    });
});

var aplicacion = constructor.Build();

await aplicacion.Services.GetRequiredService<InicializadorBaseDatos>().InicializarAsync(CancellationToken.None);

aplicacion.UseExceptionHandler();
aplicacion.UseStaticFiles();
aplicacion.UseSwagger();
aplicacion.UseSwaggerUI(opciones =>
{
    opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "Catálogo de productos v1");
    opciones.RoutePrefix = "swagger";
    opciones.DocumentTitle = "Catálogo de productos";
    opciones.InjectStylesheet("/swagger-ui/catalogo.css");
});
aplicacion.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
aplicacion.MapGet("/api-docs", () => Results.Redirect("/swagger")).ExcludeFromDescription();
aplicacion.MapGet("/health", () => Results.Ok(new { estado = "Disponible" })).ExcludeFromDescription();
aplicacion.MapControllers();

aplicacion.Run();
