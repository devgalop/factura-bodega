using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Users.Customers.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Features.Users.Customers.EditCustomer
{
    public record EditCustomerRequest(
        string Id,
        string Name,
        string Email,
        string Document
    ) : ICommand;

    public sealed class EditCustomerHandler(AppDatabaseContext dbContext) : ICommandHandler<EditCustomerRequest>
    {
        public async Task HandleAsync(EditCustomerRequest command)
        {
            var customer = await dbContext.Customers
                                    .FirstOrDefaultAsync(c => c.Id.ToString().Equals(command.Id)) 
                                    ?? throw new CustomerNotFoundException(command.Id.ToString());

            if(customer.Email != command.Email 
                || customer.Document != command.Document)
            {
                var existingCustomer =  await dbContext.Customers
                    .Where(c => (c.Email == command.Email || c.Document == command.Document) && !c.Id.ToString().Equals(command.Id))
                    .FirstOrDefaultAsync();
                
                if (existingCustomer != null) 
                    throw new CustomerAlreadyExistsException(command.Email, command.Document);
            }

            customer.Name = command.Name;
            customer.Email = command.Email;
            customer.Document = command.Document;

            dbContext.Customers.Update(customer);
            await dbContext.SaveChangesAsync();
        }
    }

    public static class EditCustomerExtensions
    {
        public static WebApplicationBuilder RegisterEditCustomerFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICommandHandler<EditCustomerRequest>, EditCustomerHandler>();
            return builder;
        }
    }
}