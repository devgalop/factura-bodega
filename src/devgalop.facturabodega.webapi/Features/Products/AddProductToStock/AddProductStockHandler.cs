using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Products.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Features.Products.AddProductToStock
{
    /// <summary>
    /// Comando para agregar stock a un producto existente
    /// </summary>
    /// <param name="ProductId">Identificador del producto</param>
    /// <param name="Quantity">Cantidad</param>
    public record AddProductStockCommand(string ProductId, int Quantity):ICommand;

    /// <summary>
    /// Handler para agregar stock a un producto existente
    /// </summary>
    /// <param name="dbContext"></param>
    public sealed class AddProductStockHandler(
        AppDatabaseContext dbContext
    ) : ICommandHandler<AddProductStockCommand>
    {
        public async Task HandleAsync(AddProductStockCommand command)
        {
            var productFound = await dbContext.Products
                                        .Include(p => p.Stock)
                                        .FirstOrDefaultAsync(p => p.Id.ToString() == command.ProductId)
                                        ?? throw new ProductNotFoundException(command.ProductId);
            if(productFound.Stock is null)
            {
                productFound.Stock = new ProductStockEntity(productFound, command.Quantity);
                dbContext.ProductStocks.Add(productFound.Stock);
                await dbContext.SaveChangesAsync();
                return;
            }
            productFound.Stock.Quantity += command.Quantity;
            dbContext.ProductStocks.Update(productFound.Stock);
            await dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Extensiones para registrar el handler de agregar stock a un producto existente
    /// </summary>
    public static class AddProductStockExtensions
    {
        /// <summary>
        /// Registra el handler para agregar stock a un producto existente
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder RegisterAddProductStockFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICommandHandler<AddProductStockCommand>, AddProductStockHandler>();
            return builder;
        }
    }
}