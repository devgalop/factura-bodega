using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using Microsoft.AspNetCore.Mvc;

namespace devgalop.facturabodega.webapi.Features.Products.GetStock
{
    public record GetStockResponse(bool IsSuccessful, string Message, int Stock);
    /// <summary>
    /// Endpoint para obtener el stock disponible de un producto específico utilizando su identificador.
    /// </summary>
    public sealed class GetProductStockEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/stock", async (
                [FromQuery] string productId,
                IMediator mediator
            ) =>
            {
                if(string.IsNullOrEmpty(productId))
                {
                    return Results.BadRequest(new GetStockResponse(
                        false, 
                        "El identificador del producto es obligatorio.",
                        0
                    ));
                }

                var result = await mediator.SendAsync<GetProductStockQuery, GetProductStockResponse>(new GetProductStockQuery(productId));

                return Results.Ok(new GetStockResponse(true, $"El stock del producto con ID '{productId}' es {result.Stock}.", result.Stock));
            })
            .RequireAuthorization(["AdminOnly","FacturadorOnly"])
            .WithName("GetStock")
            .WithSummary("Obtener Inventario de Producto")
            .WithDescription(""" 
                Obtener la cantidad de stock disponible para un producto específico utilizando su identificador.
                - `productId`: Identificador único del producto.
            """)
            .Produces<GetProductStockResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();
        }
    }
}