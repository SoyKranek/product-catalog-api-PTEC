using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.Productos;

namespace ProductCatalog.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductosController : ControllerBase
{
    private readonly CrearProductoManejador _crearProductoManejador;
    private readonly ConsultarProductoPorIdManejador _consultarProductoPorIdManejador;
    private readonly ListarProductosManejador _listarProductosManejador;
    private readonly ActualizarProductoManejador _actualizarProductoManejador;
    private readonly EliminarProductoManejador _eliminarProductoManejador;
    private readonly AjustarExistenciasProductoManejador _ajustarExistenciasProductoManejador;
    private readonly IValidator<CrearProductoComando> _validadorCrear;
    private readonly IValidator<ActualizarProductoComando> _validadorActualizar;
    private readonly IValidator<ListarProductosConsulta> _validadorListar;
    private readonly IValidator<AjustarExistenciasProductoComando> _validadorAjustar;

    public ProductosController(
        CrearProductoManejador crearProductoManejador,
        ConsultarProductoPorIdManejador consultarProductoPorIdManejador,
        ListarProductosManejador listarProductosManejador,
        ActualizarProductoManejador actualizarProductoManejador,
        EliminarProductoManejador eliminarProductoManejador,
        AjustarExistenciasProductoManejador ajustarExistenciasProductoManejador,
        IValidator<CrearProductoComando> validadorCrear,
        IValidator<ActualizarProductoComando> validadorActualizar,
        IValidator<ListarProductosConsulta> validadorListar,
        IValidator<AjustarExistenciasProductoComando> validadorAjustar)
    {
        _crearProductoManejador = crearProductoManejador;
        _consultarProductoPorIdManejador = consultarProductoPorIdManejador;
        _listarProductosManejador = listarProductosManejador;
        _actualizarProductoManejador = actualizarProductoManejador;
        _eliminarProductoManejador = eliminarProductoManejador;
        _ajustarExistenciasProductoManejador = ajustarExistenciasProductoManejador;
        _validadorCrear = validadorCrear;
        _validadorActualizar = validadorActualizar;
        _validadorListar = validadorListar;
        _validadorAjustar = validadorAjustar;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductoRespuesta), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear([FromBody] SolicitudCrearProducto solicitud, CancellationToken cancelacion)
    {
        var comando = new CrearProductoComando(solicitud.Nombre, solicitud.Descripcion, solicitud.PrecioUnitario, solicitud.ExistenciasIniciales);
        await ValidarAsync(_validadorCrear, comando, cancelacion);
        var producto = await _crearProductoManejador.EjecutarAsync(comando, cancelacion);
        return CreatedAtAction(nameof(ObtenerPorId), new { productoId = producto.ProductoId }, producto);
    }

    [HttpGet("{productoId:guid}")]
    [ProducesResponseType(typeof(ProductoRespuesta), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductoRespuesta>> ObtenerPorId(Guid productoId, CancellationToken cancelacion)
    {
        var producto = await _consultarProductoPorIdManejador.EjecutarAsync(new ConsultarProductoPorIdConsulta(productoId), cancelacion);
        return Ok(producto);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<ProductoRespuesta>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResultadoPaginado<ProductoRespuesta>>> Listar(
        [FromQuery] int numeroPagina = 1,
        [FromQuery] int tamanoPagina = 20,
        CancellationToken cancelacion = default)
    {
        var consulta = new ListarProductosConsulta(numeroPagina, tamanoPagina);
        await ValidarAsync(_validadorListar, consulta, cancelacion);
        var pagina = await _listarProductosManejador.EjecutarAsync(consulta, cancelacion);
        return Ok(pagina);
    }

    [HttpPut("{productoId:guid}")]
    [ProducesResponseType(typeof(ProductoRespuesta), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductoRespuesta>> Actualizar(
        Guid productoId,
        [FromBody] SolicitudActualizarProducto solicitud,
        CancellationToken cancelacion)
    {
        var comando = new ActualizarProductoComando(productoId, solicitud.Nombre, solicitud.Descripcion, solicitud.PrecioUnitario);
        await ValidarAsync(_validadorActualizar, comando, cancelacion);
        var producto = await _actualizarProductoManejador.EjecutarAsync(comando, cancelacion);
        return Ok(producto);
    }

    [HttpDelete("{productoId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(Guid productoId, CancellationToken cancelacion)
    {
        await _eliminarProductoManejador.EjecutarAsync(new EliminarProductoComando(productoId), cancelacion);
        return NoContent();
    }

    [HttpPatch("{productoId:guid}/stock")]
    [ProducesResponseType(typeof(ProductoRespuesta), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductoRespuesta>> AjustarExistencias(
        Guid productoId,
        [FromBody] SolicitudAjustarExistencias solicitud,
        CancellationToken cancelacion)
    {
        var comando = new AjustarExistenciasProductoComando(productoId, solicitud.CambioCantidad);
        await ValidarAsync(_validadorAjustar, comando, cancelacion);
        var producto = await _ajustarExistenciasProductoManejador.EjecutarAsync(comando, cancelacion);
        return Ok(producto);
    }

    private static async Task ValidarAsync<T>(IValidator<T> validador, T instancia, CancellationToken cancelacion)
    {
        var resultado = await validador.ValidateAsync(instancia, cancelacion);
        if (!resultado.IsValid)
        {
            throw new ValidacionException(resultado.Errors.Select(error => error.ErrorMessage).ToArray());
        }
    }
}

public sealed record SolicitudCrearProducto(string Nombre, string Descripcion, decimal PrecioUnitario, int ExistenciasIniciales);

public sealed record SolicitudActualizarProducto(string Nombre, string Descripcion, decimal PrecioUnitario);

public sealed record SolicitudAjustarExistencias(int CambioCantidad);
