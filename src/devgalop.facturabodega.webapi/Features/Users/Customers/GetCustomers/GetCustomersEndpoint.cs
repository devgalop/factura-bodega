using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;

namespace devgalop.facturabodega.webapi.Features.Users.Customers.GetCustomers
{
    public class GetCustomersEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/customers", async (
                IMediator mediator
            ) =>
            {
                var customers = await mediator.SendAsync<GetCustomersQuery, GetCustomersResponse>(new GetCustomersQuery());
                return Results.Ok(customers);
            })
            .RequireAuthorization("CanListCustomers")
            .WithName("GetCustomer")
            .WithSummary("Obtener listado de clientes")
            .WithDescription(""" 
                Obtener listado de clientes registrados en el sistema.
            """)
            .Produces<GetCustomersResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();
        }
    }
}