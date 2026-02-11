using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using FluentValidation;

namespace devgalop.facturabodega.webapi.Features.Invoices.CreateInvoice
{

    public record CreateInvoiceDetailRequest(
        string ProductId,
        int Quantity
    );

    public record CreateInvoiceRequest(
        string ClientId,
        string EmployeeId,
        IEnumerable<CreateInvoiceDetailRequest> Details
    );

    public record CreateInvoiceResponse(
        string Message
    );

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

    public sealed class CreateInvoiceDetailValidator : AbstractValidator<CreateInvoiceDetailRequest>
    {
        public CreateInvoiceDetailValidator()
        {
            RuleFor(r => r.ProductId).NotEmpty().WithMessage("El detalle de la factura debe estar relacionado a un producto.");
            RuleFor(r => r.Quantity).GreaterThan(0).WithMessage("La cantidad del detalle de la factura debe ser mayor a cero.");
        }
    }
}