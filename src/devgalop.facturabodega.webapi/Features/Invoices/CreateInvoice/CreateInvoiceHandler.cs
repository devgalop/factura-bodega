using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Invoices.Common;
using devgalop.facturabodega.webapi.Features.Products.Common;
using devgalop.facturabodega.webapi.Features.Users.Customers.GetCustomers;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Features.Invoices.CreateInvoice
{
    /// <summary>
    /// Comando para crear una factura
    /// </summary>
    /// <param name="ClientId">Identificador del cliente</param>
    /// <param name="EmployeeId">Identificador del empleado</param>
    /// <param name="Details">Detalles facturados</param>
    public record CreateInvoiceCommand(
        string ClientId,
        string EmployeeId,
        IEnumerable<CreateInvoiceDetailRequest> Details
    ) : ICommand;

    /// <summary>
    /// Manejador para el comando de crear una factura
    /// </summary>
    /// <param name="dbContext"></param>
    public sealed class CreateInvoiceHandler(
        AppDatabaseContext dbContext
    ) : ICommandHandler<CreateInvoiceCommand>
    {
        public async Task HandleAsync(CreateInvoiceCommand command)
        {
            List<ValidationFailure> validationFailures = [];
            //TODO: Validar existencia de cliente
            var customer = await dbContext.Customers
                                            .FirstOrDefaultAsync(c => c.Id.ToString() == command.ClientId);
            if (customer == null) 
            {
                validationFailures.Add(
                    new ValidationFailure(
                        nameof(command.ClientId),
                        "El cliente no se encuentra registrado en la base de datos."
                    )
                );
            }
            //TODO: Validar existencia de empleado
            var employee = await dbContext.Employees
                                            .FirstOrDefaultAsync(e => e.Id.ToString() == command.EmployeeId);
            if(employee == null)
            {
                validationFailures.Add(
                    new ValidationFailure(
                        nameof(command.EmployeeId),
                        "El empleado no se encuentra registrado en la base de datos."
                    )
                );
            }
            InvoiceEntity invoice = new(customer!, employee!);
            //TODO: Validar existencia de productos con stock suficiente
            List<InvoiceDetailEntity> invoiceDetails = [];
            foreach (CreateInvoiceDetailRequest detail in command.Details)
            {
                var product = await dbContext.Products.Include(p => p.Stock)
                                                    .FirstOrDefaultAsync(p => p.Id.ToString() == detail.ProductId);
                if(product == null)
                {
                    validationFailures.Add(
                        new ValidationFailure(
                            nameof(detail.ProductId),
                            $"El producto con Id {detail.ProductId} no se encuentra registrado en la base de datos."
                        )
                    );
                    continue;
                }
                if(product.Stock.Quantity < detail.Quantity)
                {
                    validationFailures.Add(
                        new ValidationFailure(
                            nameof(detail.Quantity),
                            $"El producto con Id {detail.ProductId} no tiene stock suficiente para la cantidad solicitada."
                        )
                    );
                    continue;
                }
                invoiceDetails.Add(new InvoiceDetailEntity(detail.Quantity, product, invoice));
            }

            if(validationFailures.Count != 0) throw new ValidationException(validationFailures);
            //TODO: Crear la factura y sus detalles
            dbContext.Invoices.Add(invoice);
            dbContext.InvoiceDetails.AddRange(invoiceDetails);

            //TODO: Actualizar el stock de los productos facturados
            foreach (InvoiceDetailEntity detail in invoiceDetails)
            {
                detail.Product.Stock.Quantity -= detail.Quantity;
                dbContext.ProductStocks.Update(detail.Product.Stock);
            }

            await dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Extensiones para registrar la funcionalidad de crear una factura
    /// </summary>
    public static class CreateInvoiceExtensions
    {
        /// <summary>
        /// Registra la funcionalidad de crear una factura
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder RegisterCreateInvoiceFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICommandHandler<CreateInvoiceCommand>, CreateInvoiceHandler>();
            return builder;
        }
    }
}