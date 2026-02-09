using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using FluentValidation;

namespace devgalop.facturabodega.webapi.Features.Products.UpdateStock
{
    /// <summary>
    /// Peticion para actualizar el número de unidades en el stock de un producto
    /// </summary>
    /// <param name="ProductId">Identificador del producto</param>
    /// <param name="Quantity">Cantidad</param>
    public record UpdateStockRequest(string ProductId, int Quantity);

    /// <summary>
    /// Respuesta de la petición para actualizar el número de unidades en el stock de un producto
    /// </summary>
    /// <param name="IsSuccessful">Resultado de la operación</param>
    /// <param name="Message">Mensaje descriptivo</param>
    public record UpdateStockResponse(bool IsSuccessful, string Message);

    /// <summary>
    /// Endpoint para actualizar el número de unidades en el stock de un producto
    /// </summary>
    public class UpdateStockEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/products/update-stock", async (
                UpdateStockRequest request,
                IMediator mediator,
                IValidator<UpdateStockRequest> validator
            ) =>
            {
                validator.ValidateAndThrow(request);
                await mediator.SendAsync(new UpdateStockCommand(request.ProductId, request.Quantity));
                return Results.Ok(new UpdateStockResponse(true, "Stock actualizado correctamente."));
            })
            .RequireAuthorization("CanEditProduct")
            .WithName("EditStock")
            .WithSummary("Edita las existencias dentro del Stock")
            .WithDescription(""" 
                Edita existencias de un producto con la cantidad especificada.
                - `ProductId`: Identificador del producto al que se le agregará stock.
                - `Quantity`: Cantidad a agregar al stock del producto.
            """)
            .Produces<UpdateStockResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesValidationProblem();
        }
    }

    /// <summary>
    /// Validador para la petición de actualización del stock de un producto
    /// </summary>
    public sealed class UpdateStockValidator : AbstractValidator<UpdateStockRequest>
    {
        public UpdateStockValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("El identificador del producto es obligatorio.");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("La cantidad debe ser mayor o igual a cero.");
        }
    }
}