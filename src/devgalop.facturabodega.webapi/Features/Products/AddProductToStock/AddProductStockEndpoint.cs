using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using FluentValidation;

namespace devgalop.facturabodega.webapi.Features.Products.AddProductToStock
{
    /// <summary>
    /// Peticion para agregar stock a un producto existente
    /// </summary>
    /// <param name="ProductId">Identificador del producto</param>
    /// <param name="Quantity">Cantidad</param>
    public record AddProductStockRequest(string ProductId, int Quantity);

    /// <summary>
    /// Respuesta para la petición de agregar stock a un producto existente
    /// </summary>
    /// <param name="IsSuccessful">Resultado de la operación</param>
    /// <param name="Message">Mensaje descriptivo</param>
    public record AddProductStockResponse(bool IsSuccessful, string Message);

    /// <summary>
    /// Endpoint para agregar stock a un producto existente
    /// </summary>
    public sealed class AddProductStockEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/products/stock", async (
                AddProductStockRequest request,
                IMediator mediator,
                IValidator<AddProductStockRequest> validator
            )=>
            {
                validator.ValidateAndThrow(request);
                await mediator.SendAsync(new AddProductStockCommand(request.ProductId, request.Quantity));                
                return Results.Ok(new AddProductStockResponse(true, "Stock agregado exitosamente."));
            })
            .RequireAuthorization("CanEditProduct")
            .WithName("AddProductStock")
            .WithSummary("Agrega Existencias del Producto al Stock")
            .WithDescription(""" 
                Agrega al stock del producto existente la cantidad especificada.
                - `ProductId`: Identificador del producto al que se le agregará stock.
                - `Quantity`: Cantidad a agregar al stock del producto.
            """)
            .Produces<AddProductStockResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesValidationProblem();
        }
    }

    /// <summary>
    /// Validador para la petición de agregar stock a un producto existente
    /// </summary>
    public sealed class AddProductStockValidator : AbstractValidator<AddProductStockRequest>
    {
        public AddProductStockValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("El identificador del producto es requerido.");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("La cantidad debe ser mayor que cero.");
        }
    }
}