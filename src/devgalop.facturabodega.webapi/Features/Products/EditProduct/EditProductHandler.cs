using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Products.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Features.Products.EditProduct
{
    /// <summary>
    /// Comando para editar un producto
    /// </summary>
    /// <param name="Id">Identificador del producto</param>
    /// <param name="Name">Nombre del producto</param>
    /// <param name="Description">Descripción del producto</param>
    /// <param name="Price">Precio unitario</param>
    public record EditProductCommand(
        string Id, 
        string Name, 
        string Description, 
        float Price
    ): ICommand;

    /// <summary>
    /// Handler para editar un producto
    /// </summary>
    /// <param name="dbContext"></param>
    public sealed class EditProductHandler(
        AppDatabaseContext dbContext
    ) : ICommandHandler<EditProductCommand>
    {
        public async Task HandleAsync(EditProductCommand command)
        {
            var productFound = await dbContext.Products
                                    .FirstOrDefaultAsync(p => p.Id.ToString() == command.Id)
                                    ?? throw new ProductNotFoundException(command.Id);
            productFound.Name = command.Name;
            productFound.Description = command.Description;
            productFound.UnitPrice = command.Price;
            dbContext.Products.Update(productFound);
            await dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Extensiones para registrar el handler de editar producto
    /// </summary>
    public static class EditProductExtensions
    {
        /// <summary>
        /// Registra el handler para editar un producto
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder RegisterEditProductFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICommandHandler<EditProductCommand>, EditProductHandler>();
            return builder;
        }
    }
}