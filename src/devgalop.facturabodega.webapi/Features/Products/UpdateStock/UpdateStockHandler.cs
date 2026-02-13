using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Products.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Features.Products.UpdateStock
{
    /// <summary>
    /// Comando para actualizar el número de unidades en el stock de un producto
    /// </summary>
    /// <param name="ProductId">Identificador del producto</param>
    /// <param name="Quantity">Cantidad</param>
    public record UpdateStockCommand(string ProductId, int Quantity): ICommand;

    /// <summary>
    /// Manejador para el comando de actualización del stock de un producto
    /// </summary>
    public sealed class UpdateStockHandler(
        AppDatabaseContext dbContext
    ) : ICommandHandler<UpdateStockCommand>
    {
        public async Task HandleAsync(UpdateStockCommand command)
        {
            var productFound = await dbContext.Products
                                            .Include(p => p.Stock)
                                            .FirstOrDefaultAsync(p => p.Id.ToString() == command.ProductId)
                                            ?? throw new ProductNotFoundException(command.ProductId);
            
            if(productFound.Stock is null) throw new UnassignmentStockException(command.ProductId);

            productFound.Stock.Quantity = command.Quantity;
            dbContext.ProductStocks.Update(productFound.Stock);
            await dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Comando para remover una cantidad del stock de un producto
    /// </summary>
    /// <param name="ProductId">Identificador del producto</param>
    /// <param name="Quantity">Cantidad</param>
    public record RemoveFromStockCommand(string ProductId, int Quantity): ICommand;

    /// <summary>
    /// Manejador para el comando de remover una cantidad del stock de un producto
    /// </summary>
    /// <param name="dbContext"></param>
    public sealed class RevomeFromStockHandler(
        AppDatabaseContext dbContext
    ) : ICommandHandler<RemoveFromStockCommand>
    {
        public async Task HandleAsync(RemoveFromStockCommand command)
        {
            var productFound = await dbContext.Products
                                            .Include(p => p.Stock)
                                            .FirstOrDefaultAsync(p => p.Id.ToString() == command.ProductId)
                                            ?? throw new ProductNotFoundException(command.ProductId);
            
            if(productFound.Stock is null) throw new UnassignmentStockException(command.ProductId);

            productFound.Stock.Quantity -= command.Quantity;
            dbContext.ProductStocks.Update(productFound.Stock);
            await dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Extensiones para la funcionalidad de actualización del stock de un producto
    /// </summary>
    public static class UpdateStockExtensions
    {
        /// <summary>
        /// Registra los servicios necesarios para la funcionalidad de actualización del stock de un producto
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder RegisterUpdateStockFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICommandHandler<UpdateStockCommand>, UpdateStockHandler>()
                            .AddScoped<ICommandHandler<RemoveFromStockCommand>, RevomeFromStockHandler>();
            return builder;
        }
    }
}