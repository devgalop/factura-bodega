using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Products.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Features.Products.AddProduct
{
    /// <summary>
    /// Comando para agregar un nuevo producto.
    /// </summary>
    /// <param name="Name">Nombre del producto</param>
    /// <param name="Description">Descripción del producto</param>
    /// <param name="UnitPrice">Precio unitario</param>
    public record AddProductCommand(string Name, string Description, float UnitPrice): ICommand;

    /// <summary>
    /// Manejador para el comando de agregar un nuevo producto.
    /// </summary>
    /// <param name="dbContext"></param>
    public class AddProductHandler(AppDatabaseContext dbContext) : ICommandHandler<AddProductCommand>
    {
        public async Task HandleAsync(AddProductCommand command)
        {
            var productFound = await dbContext.Products
                                            .FirstOrDefaultAsync(p => p.Name == command.Name);
            
            if(productFound != null) throw new ExistingProductException(command.Name);
            var newProduct = new ProductEntity(command.Name, command.Description, command.UnitPrice);
            await dbContext.Products.AddAsync(newProduct);
            await dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Dependencias necesarias para la funcionalidad de agregar productos.
    /// </summary>
    public static class AddProductExtensions
    {
        /// <summary>
        /// Registra las dependencias necesarias para la funcionalidad de agregar productos.
        /// </summary>
        /// <param name="builder">Builder de aplicacion</param>
        /// <returns></returns>
        public static WebApplicationBuilder RegisterAddProductFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICommandHandler<AddProductCommand>, AddProductHandler>();
            return builder;
        }
    }
}