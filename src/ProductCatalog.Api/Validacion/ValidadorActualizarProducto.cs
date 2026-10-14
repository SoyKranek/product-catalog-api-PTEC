using FluentValidation;
using ProductCatalog.Application.Productos;

namespace ProductCatalog.Api.Validacion;

public sealed class ValidadorActualizarProducto : AbstractValidator<ActualizarProductoComando>
{
    public ValidadorActualizarProducto()
    {
        RuleFor(comando => comando.ProductoId).NotEmpty().WithMessage("El identificador del producto es obligatorio.");
        RuleFor(comando => comando.Nombre).NotEmpty().MaximumLength(120)
            .WithMessage("El nombre es obligatorio y admite como máximo 120 caracteres.");
        RuleFor(comando => comando.Descripcion).NotNull().MaximumLength(1000)
            .WithMessage("La descripción admite como máximo 1000 caracteres.");
        RuleFor(comando => comando.PrecioUnitario).GreaterThanOrEqualTo(0)
            .WithMessage("El precio unitario no puede ser negativo.");
        RuleFor(comando => comando.PrecioUnitario).Must(TieneComoMaximoDosDecimales)
            .WithMessage("El precio unitario admite como máximo dos decimales.");
    }

    private static bool TieneComoMaximoDosDecimales(decimal precioUnitario)
    {
        return decimal.Round(precioUnitario, 2, MidpointRounding.AwayFromZero) == precioUnitario;
    }
}
