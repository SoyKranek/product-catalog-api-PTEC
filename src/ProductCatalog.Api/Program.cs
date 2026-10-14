using FluentValidation;
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
    opciones.SwaggerDoc("v1", new() { Title = "Catálogo de productos", Version = "v1" });
});

var aplicacion = constructor.Build();

await aplicacion.Services.GetRequiredService<InicializadorBaseDatos>().InicializarAsync(CancellationToken.None);

aplicacion.UseExceptionHandler();
aplicacion.UseSwagger();
aplicacion.UseSwaggerUI(opciones =>
{
    opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "Catálogo de productos v1");
    opciones.RoutePrefix = "swagger";
});
aplicacion.MapGet("/", () => Results.Redirect("/swagger"));
aplicacion.MapGet("/api-docs", () => Results.Redirect("/swagger"));
aplicacion.MapGet("/health", () => Results.Ok(new { estado = "Disponible" }));
aplicacion.MapControllers();

aplicacion.Run();
