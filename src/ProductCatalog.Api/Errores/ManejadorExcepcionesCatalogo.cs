using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.Productos;
using ProductCatalog.Domain.Productos;

namespace ProductCatalog.Api.Errores;

public sealed class ManejadorExcepcionesCatalogo : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext contextoHttp,
        Exception excepcion,
        CancellationToken cancelacion)
    {
        var problema = excepcion switch
        {
            ValidacionException validacion => ProblemaValidacion(validacion),
            AjusteExistenciasInvalidoException => Problema(StatusCodes.Status400BadRequest, "Ajuste de existencias inválido", excepcion.Message),
            ExistenciasInsuficientesException => Problema(StatusCodes.Status400BadRequest, "Existencias insuficientes", excepcion.Message),
            ArgumentException => Problema(StatusCodes.Status400BadRequest, "Producto inválido", excepcion.Message),
            ProductoNoEncontradoException => Problema(StatusCodes.Status404NotFound, "Producto no encontrado", excepcion.Message),
            ConflictoExistenciasException => Problema(StatusCodes.Status409Conflict, "Conflicto de existencias", excepcion.Message),
            _ => null
        };

        if (problema is null)
        {
            return false;
        }

        contextoHttp.Response.StatusCode = problema.Status ?? StatusCodes.Status500InternalServerError;
        await contextoHttp.Response.WriteAsJsonAsync(problema, cancelacion);
        return true;
    }

    private static ProblemDetails ProblemaValidacion(ValidacionException validacion)
    {
        var problema = Problema(StatusCodes.Status400BadRequest, "Validación fallida", validacion.Message);
        problema.Extensions["errores"] = validacion.Errores;
        return problema;
    }

    private static ProblemDetails Problema(int codigoEstado, string titulo, string detalle)
    {
        return new ProblemDetails
        {
            Status = codigoEstado,
            Title = titulo,
            Detail = detalle,
            Type = $"https://httpstatuses.com/{codigoEstado}"
        };
    }
}
