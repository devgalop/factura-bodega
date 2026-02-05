using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using FluentValidation;

namespace devgalop.facturabodega.webapi.Features.Products.AddProduct
{

    public record AddProductRequest(string Name, string Description, float Price);

    public record AddProductResponse(bool IsSuccessful, string Message);

    public class AddProductEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/products", async (
                AddProductRequest request,
                IMediator mediator, 
                IValidator<AddProductRequest> validator
            ) =>
            {
                validator.ValidateAndThrow(request);
                await mediator.SendAsync(new AddProductCommand(
                    request.Name,
                    request.Description,
                    request.Price
                ));
                return Results.Json(new AddProductResponse(true, "Producto agregado exitosamente."), statusCode: 201);
            });
        }
    }

    public sealed class AddProductRequestValidator : AbstractValidator<AddProductRequest>
    {
        public AddProductRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del producto es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre del producto no puede exceder los 100 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("La descripción del producto no puede exceder los 500 caracteres.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("El precio unitario debe ser mayor que cero.");
        }
    }
}