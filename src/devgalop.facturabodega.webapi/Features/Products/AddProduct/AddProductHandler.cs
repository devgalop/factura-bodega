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
    public record AddProductCommand(string Name, string Description, float UnitPrice): ICommand;

    public class AddProductHandler(AppDatabaseContext dbContext) : ICommandHandler<AddProductCommand>
    {
        public async Task HandleAsync(AddProductCommand command)
        {
            var productFound = await dbContext.Products
                                            .FirstOrDefaultAsync(p => p.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase));
            
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
        public static WebApplicationBuilder RegisterAddProductFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICommandHandler<AddProductCommand>, AddProductHandler>();
            return builder;
        }
    }
}