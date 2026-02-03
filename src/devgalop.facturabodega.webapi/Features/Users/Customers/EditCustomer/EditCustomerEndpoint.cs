using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using FluentValidation;

namespace devgalop.facturabodega.webapi.Features.Users.Customers.EditCustomer
{
    public record EditCustomerResponse(bool IsSuccessful, string Message);

    public class EditCustomerEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/customers", async (
                EditCustomerRequest request,
                IMediator mediator,
                IValidator<EditCustomerRequest> validator
            ) =>
            {
                validator.ValidateAndThrow(request);
                await mediator.SendAsync(request);
                return Results.Json(new EditCustomerResponse(true, "El cliente fue actualizado en la base de datos"), statusCode: 200);
            })
            .RequireAuthorization(["AdminOnly","FacturadorOnly"])
            .WithName("EditCustomer")
            .WithSummary("Editar Cliente")
            .WithDescription(""" 
                Edita la información de un cliente en el sistema con la información proporcionada.
                - `Id`: Identificador del cliente.
                - `Name`: Nombre completo del cliente.
                - `Email`: Correo electrónico del cliente.
                - `Document`: Documento del cliente.
            """)
            .Produces<EditCustomerResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();
        }
    }

    public sealed class EditCustomerRequestValidator : AbstractValidator<EditCustomerRequest>
    {
        public EditCustomerRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El Id del usuario es obligatorio.");
            
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del usuario es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre del usuario no puede exceder los 100 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo del usuario es obligatorio.")
                .EmailAddress().WithMessage("El correo debe ser valido.")
                .MaximumLength(100).WithMessage("El correo no puede exceder los 100 caracteres.");

            RuleFor(x => x.Document)
                .NotEmpty().WithMessage("El documento del usuario es obligatorio.")
                .MaximumLength(50).WithMessage("El documento del usuario no puede exceder los 50 caracteres.");
        }
    }
}