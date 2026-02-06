using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Products.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Features.Products.GetStock
{
    /// <summary>
    /// Consulta para obtener el stock disponible de un producto específico utilizando su identificador.
    /// </summary>
    /// <param name="ProductId">Identificador de producto</param>
    public record GetProductStockQuery(string ProductId): IQuery;
    /// <summary>
    /// Respuesta al obtener el stock disponible de un producto específico utilizando su identificador.
    /// </summary>
    /// <param name="ProductId">Identificador de producto</param>
    /// <param name="Stock">Cantidad disponible</param>
    public record GetProductStockResponse(string ProductId, int Stock);

    /// <summary>
    /// Manejador para obtener el stock disponible de un producto específico utilizando su identificador.
    /// </summary>
    /// <param name="dbContext"></param>
    public sealed class GetProductStockHandler(AppDatabaseContext dbContext) : IQueryHandler<GetProductStockQuery, GetProductStockResponse>
    {
        public async Task<GetProductStockResponse> HandleAsync(GetProductStockQuery query)
        {
            var productFound = await dbContext.Products
                                            .Include(p => p.Stock)
                                            .Where(p => p.Id.ToString() == query.ProductId)
                                            .Select(ps => new GetProductStockResponse(
                                                ps.Id.ToString(), 
                                                ps.Stock != null ? ps.Stock.Quantity : 0
                                            ))
                                            .FirstOrDefaultAsync() 
                                            ?? throw new ProductNotFoundException(query.ProductId);

            return productFound;
        }
    }

    /// <summary>
    /// Extensiones para registrar la característica de obtener el stock disponible de un producto específico utilizando su identificador.
    /// </summary>
    public static class GetProductStockExtensions
    {
        /// <summary>
        /// Registra la característica de obtener el stock disponible de un producto específico utilizando su identificador en el contenedor de servicios.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder RegisterGetProductStockFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IQueryHandler<GetProductStockQuery, GetProductStockResponse>, GetProductStockHandler>();
            return builder;
        }
    }
}