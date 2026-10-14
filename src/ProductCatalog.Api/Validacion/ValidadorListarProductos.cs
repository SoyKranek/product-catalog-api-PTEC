using FluentValidation;
using ProductCatalog.Application.Productos;

namespace ProductCatalog.Api.Validacion;

public sealed class ValidadorListarProductos : AbstractValidator<ListarProductosConsulta>
{
    public ValidadorListarProductos()
    {
        RuleFor(consulta => consulta.NumeroPagina).GreaterThanOrEqualTo(1)
            .WithMessage("El número de página debe ser mayor o igual a 1.");
        RuleFor(consulta => consulta.TamanoPagina).InclusiveBetween(1, 100)
            .WithMessage("El tamaño de página debe estar entre 1 y 100.");
    }
}
