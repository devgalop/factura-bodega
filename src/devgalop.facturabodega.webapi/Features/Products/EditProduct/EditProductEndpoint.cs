using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace devgalop.facturabodega.webapi.Features.Products.EditProduct
{
    /// <summary>
    /// Peticion para editar un producto
    /// </summary>
    /// <param name="Id">Identificador del producto</param>
    /// <param name="Name">Nombre del producto</param>
    /// <param name="Description">Descripción del producto</param>
    /// <param name="Price">Precio Unitario</param>
    public record EditProductRequest(
        string Id, 
        string Name, 
        string Description, 
        float Price
    );

    /// <summary>
    /// Respuesta después de editar un producto
    /// </summary>
    /// <param name="IsSuccessful">Respuesta del proceso</param>
    /// <param name="Message">Mensaje descriptivo</param>
    public record EditProductResponse(bool IsSuccessful, string Message);

    /// <summary>
    /// Endpoint para editar un producto
    /// </summary>
    public sealed class EditProductEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/products", async (
                EditProductRequest request,
                IMediator mediator,
                IValidator<EditProductRequest> validator
            ) =>
            {
                validator.ValidateAndThrow(request);
                await mediator.SendAsync(new EditProductCommand(
                    request.Id,
                    request.Name,
                    request.Description,
                    request.Price
                ));
                return Results.Json(new EditProductResponse(true, "Producto editado exitosamente."), statusCode: 200);
            })
            .RequireAuthorization("CanEditProduct")
            .WithName("EditProduct")
            .WithSummary("Editar Producto Existente")
            .WithDescription(""" 
                Edita un producto existente con la información proporcionada.
                - `Id`: Identificador del producto a editar.
                - `Name`: Nombre del producto.
                - `Description`: Descripcion del producto.
                - `Price`: Precio unitario.
            """)
            .Produces<EditProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesValidationProblem();
        }
    }

    /// <summary>
    /// Validador para la petición de editar un producto
    /// </summary>
    public sealed class EditProductRequestValidator : AbstractValidator<EditProductRequest>
    {
        public EditProductRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("El Id del producto es requerido");

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