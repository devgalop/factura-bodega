using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Users.Customers.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Features.Users.Customers.AddCustomer
{
    /// <summary>
    /// Solicitud para agregar un nuevo cliente.
    /// </summary>
    /// <param name="Name">Nombre del cliente</param>
    /// <param name="Email">Correo del cliente</param>
    /// <param name="Document">Documento del cliente</param>
    public record AddCustomerRequest
    (
        string Name,
        string Email,
        string Document
    ): ICommand;


    /// <summary>
    /// Manejador para agregar un nuevo cliente.
    /// </summary>
    /// <param name="dbContext"></param>
    public class AddCustomerHandler(AppDatabaseContext dbContext) : ICommandHandler<AddCustomerRequest>
    {
        public async Task HandleAsync(AddCustomerRequest command)
        {
            var customerFound = await dbContext.Customers
                .Where(c => c.Document == command.Document || c.Email == command.Email)
                .FirstOrDefaultAsync();
            
            if (customerFound != null) throw new CustomerAlreadyExistsException(command.Email, command.Document);

            var customer = new CustomerEntity(command.Name, command.Email, command.Document);
            dbContext.Customers.Add(customer);
            await dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Extensiones para registrar el feature de agregar cliente.
    /// </summary>
    public static class AddCustomerExtensions
    {
        /// <summary>
        /// Registra el feature de agregar cliente.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder RegisterAddCustomerFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICommandHandler<AddCustomerRequest>, AddCustomerHandler>();
            return builder;
        }
    }
}