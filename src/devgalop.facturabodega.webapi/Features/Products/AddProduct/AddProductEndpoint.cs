using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using FluentValidation;

namespace devgalop.facturabodega.webapi.Features.Products.AddProduct
{
    /// <summary>
    /// Solicitud para agregar un nuevo producto.
    /// </summary>
    /// <param name="Name">Nombre del producto</param>
    /// <param name="Description">Descripcion del producto</param>
    /// <param name="Price">Precio unitario</param>
    public record AddProductRequest(string Name, string Description, float Price);

    /// <summary>
    /// Respuesta después de agregar un nuevo producto.
    /// </summary>
    /// <param name="IsSuccessful">Resultado de proceso</param>
    /// <param name="Message">Mensaje descriptivo</param>
    public record AddProductResponse(bool IsSuccessful, string Message);

    /// <summary>
    /// Endpoint para agregar un nuevo producto.
    /// </summary>
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
            })
            .RequireAuthorization("CanCreateProduct")
            .WithName("AddProduct")
            .WithSummary("Agregar Producto")
            .WithDescription(""" 
                Agrega un nuevo producto al sistema con la información proporcionada.
                - `Name`: Nombre del producto.
                - `Description`: Descripcion del producto.
                - `Price`: Precio unitario.
            """)
            .Produces<AddProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesValidationProblem();
        }
    }

    /// <summary>
    /// Validador para la solicitud de agregar un producto.
    /// </summary>
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