using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Users.Customers.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Features.Users.Customers.GetCustomers
{
    public record GetCustomersQuery():IQuery;
    public record GetCustomerDetail(string Id, string Name);
    public record GetCustomersResponse(List<GetCustomerDetail> Customers);

    public sealed class GetCustomersHandler(AppDatabaseContext dbContext)
                : IQueryHandler<GetCustomersQuery, GetCustomersResponse>
    {
        public async Task<GetCustomersResponse> HandleAsync(GetCustomersQuery query)
        {
            var customers = dbContext.Customers
                .Select(c => new GetCustomerDetail(c.Id.ToString(), c.Name))
                .ToList();
            return new GetCustomersResponse(customers);
        }
    }

    public record GetCustomerQuery(string Id):IQuery;
    
    public sealed class GetCustomerHandler(AppDatabaseContext dbContext): IQueryHandler<GetCustomerQuery, GetCustomerDetail>
    {
        public async Task<GetCustomerDetail> HandleAsync(GetCustomerQuery query)
        {
            var customer = await dbContext.Customers
                                        .FirstOrDefaultAsync(c => c.Id.ToString() == query.Id)
                                        ?? throw new CustomerNotFoundException(query.Id);
            return new GetCustomerDetail(customer.Id.ToString(), customer.Name);
        }
    }

    public static class GetCustomersExtensions
    {
        public static WebApplicationBuilder RegisterGetCustomersFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IQueryHandler<GetCustomersQuery, GetCustomersResponse>, GetCustomersHandler>()
                            .AddScoped<IQueryHandler<GetCustomerQuery, GetCustomerDetail>, GetCustomerHandler>();
            return builder;
        }
    }
}