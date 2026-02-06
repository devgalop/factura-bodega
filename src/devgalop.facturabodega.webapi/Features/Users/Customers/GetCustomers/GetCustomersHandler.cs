using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;

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

    public static class GetCustomersExtensions
    {
        public static WebApplicationBuilder RegisterGetCustomersFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IQueryHandler<GetCustomersQuery, GetCustomersResponse>, GetCustomersHandler>();
            return builder;
        }
    }
}