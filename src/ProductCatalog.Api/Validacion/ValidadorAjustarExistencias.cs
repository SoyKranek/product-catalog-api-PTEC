using FluentValidation;
using ProductCatalog.Application.Productos;

namespace ProductCatalog.Api.Validacion;

public sealed class ValidadorAjustarExistencias : AbstractValidator<AjustarExistenciasProductoComando>
{
    public ValidadorAjustarExistencias()
    {
        RuleFor(comando => comando.ProductoId).NotEmpty().WithMessage("El identificador del producto es obligatorio.");
        RuleFor(comando => comando.CambioCantidad).NotEqual(0)
            .WithMessage("El cambio de cantidad debe ser distinto de cero.");
    }
}
