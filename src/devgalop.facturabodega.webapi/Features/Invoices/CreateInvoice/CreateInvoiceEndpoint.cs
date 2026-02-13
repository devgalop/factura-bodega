using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using FluentValidation;

namespace devgalop.facturabodega.webapi.Features.Invoices.CreateInvoice
{
    /// <summary>
    /// Detalles de la factura
    /// </summary>
    /// <param name="ProductId">Identificador del producto</param>
    /// <param name="Quantity">Cantidad facturada</param>
    public record CreateInvoiceDetailRequest(
        string ProductId,
        int Quantity
    );

    /// <summary>
    /// Peticion para crear una factura
    /// </summary>
    /// <param name="ClientId">Identificador del cliente</param>
    /// <param name="EmployeeId">Identificador del empleado</param>
    /// <param name="Details">Detalles facturados</param>
    public record CreateInvoiceRequest(
        string ClientId,
        string EmployeeId,
        IEnumerable<CreateInvoiceDetailRequest> Details
    );

    /// <summary>
    /// Respuesta al crear una factura
    /// </summary>
    /// <param name="Message">Mensaje descriptivo</param>
    public record CreateInvoiceResponse(
        string Message
    );

    /// <summary>
    /// Endpoint para crear una factura
    /// </summary>
    public sealed class CreateInvoiceEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/invoices", async (
                CreateInvoiceRequest request,
                IMediator mediator,
                IValidator<CreateInvoiceRequest> validator
            ) =>
            {
                validator.ValidateAndThrow(request);
                await mediator.SendAsync(new CreateInvoiceCommand(
                    request.ClientId,
                    request.EmployeeId,
                    request.Details
                ));
                return Results.Json(new CreateInvoiceResponse("Factura creada exitosamente."), statusCode: 201); 
            })
            .RequireAuthorization("CanCreateInvoice")
            .WithName("CreateInvoice")
            .WithSummary("Crear Factura")
            .WithDescription(""" 
                Agrega una nueva factura al sistema con la información proporcionada.
                - `ClientId`: Id del cliente relacionado a la factura.
                - `EmployeeId`: Id del empleado relacionado a la factura.
                - `Details`: Detalles de la factura, cada detalle debe contener el Id del producto y la cantidad.
            """)
            .Produces<CreateInvoiceResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesValidationProblem();
        }
    }

    /// <summary>
    /// Validador para la peticion de crear una factura
    /// </summary>
    public sealed class CreateInvoiceValidator : AbstractValidator<CreateInvoiceRequest>
    {
        public CreateInvoiceValidator()
        {
            RuleFor(r => r.ClientId).NotEmpty().WithMessage("La factura debe estar relacionada a un cliente.")
                                    .MaximumLength(200).WithMessage("El Id del cliente no puede exceder los 200 caracteres.");
            RuleFor(r => r.EmployeeId).NotEmpty().WithMessage("La factura debe estar relacionada a un empleado.")
                                     .MaximumLength(200).WithMessage("El Id del empleado no puede exceder los 200 caracteres.");
            RuleFor(r => r.Details).NotEmpty().WithMessage("La factura debe tener al menos un detalle.");
            RuleForEach(r => r.Details).SetValidator(new CreateInvoiceDetailValidator());
        }
    }

    /// <summary>
    /// Validador para los detalles de la factura
    /// </summary>
    public sealed class CreateInvoiceDetailValidator : AbstractValidator<CreateInvoiceDetailRequest>
    {
        public CreateInvoiceDetailValidator()
        {
            RuleFor(r => r.ProductId).NotEmpty().WithMessage("El detalle de la factura debe estar relacionado a un producto.");
            RuleFor(r => r.Quantity).GreaterThan(0).WithMessage("La cantidad del detalle de la factura debe ser mayor a cero.");
        }
    }
}