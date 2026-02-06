using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;

namespace devgalop.facturabodega.webapi.Features.Products.GetProducts
{
    public class GetProductsEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async (IMediator mediator) =>
            {
                var result = await mediator.SendAsync<GetProductsRequest, GetProductsResponse>(new GetProductsRequest());
                return Results.Ok(result);
            })
            .RequireAuthorization("CanListProducts")
            .WithName("ListProducts")
            .WithSummary("Listar Productos")
            .WithDescription(""" 
               Lista todos los productos disponibles en el sistema con su información básica.
            """)
            .Produces<GetProductsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesValidationProblem();
        }
    }
}