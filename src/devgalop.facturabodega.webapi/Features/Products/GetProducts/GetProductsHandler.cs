using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Features.Products.GetProducts
{
    public record GetProductsRequest(): IQuery;

    public record ProductDetail(string Id, string Name, float Price, int Stock);

    public record GetProductsResponse(List<ProductDetail> Products);

    public class GetProductsHandler(AppDatabaseContext dbContext)
                : IQueryHandler<GetProductsRequest, GetProductsResponse>
    {
        public async Task<GetProductsResponse> HandleAsync(GetProductsRequest query)
        {
            var products = await dbContext.Products
                                        .Include(p => p.Stock)
                                        .Select(p => new ProductDetail(
                                            p.Id.ToString(),
                                            p.Name,
                                            p.UnitPrice,
                                            p.Stock != null ? p.Stock.Quantity : 0
                                        ))
                                        .ToListAsync();

            return new GetProductsResponse(products);
        }
    }

    public static class GetProductsExtensions
    {
        public static WebApplicationBuilder RegisterGetProductsFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IQueryHandler<GetProductsRequest, GetProductsResponse>, GetProductsHandler>();
            return builder;
        }
    }
}