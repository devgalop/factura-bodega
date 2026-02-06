using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using FluentValidation;

namespace devgalop.facturabodega.webapi.Features.Users.Customers.AddCustomer
{
    /// <summary>
    /// Respuesta al agregar un nuevo cliente.
    /// </summary>
    /// <param name="IsSuccessful">Resultado del proceso</param>
    /// <param name="Message">Mensaje descriptivo</param>
    public record AddCustomerResponse(bool IsSuccessful, string Message);

    /// <summary>
    /// Endpoint para agregar un nuevo cliente.
    /// </summary>
    public class AddCustomerEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/customers/add", async (
                AddCustomerRequest request,
                IMediator mediator,
                IValidator<AddCustomerRequest> validator
            ) =>
            {
                validator.ValidateAndThrow(request);
                await mediator.SendAsync(request);
                return Results.Json(new AddCustomerResponse(true, "El cliente fue registrado en la base de datos"), statusCode:202);
            })
            .RequireAuthorization("CanCreateCustomer")
            .WithName("AddCustomer")
            .WithSummary("Agregar Cliente")
            .WithDescription(""" 
                Agrega un nuevo cliente al sistema con la información proporcionada.
                - `Name`: Nombre completo del cliente.
                - `Email`: Correo electrónico del cliente.
                - `Document`: Documento del cliente.
            """)
            .Produces<AddCustomerResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();
        }
    }

    /// <summary>
    /// Validador para la solicitud de agregar un cliente.
    /// </summary>
    public class AddCustomerRequestValidator : AbstractValidator<AddCustomerRequest>
    {
        public AddCustomerRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Document)
                .NotEmpty().WithMessage("Document is required.")
                .MaximumLength(50).WithMessage("Document cannot exceed 50 characters.");
        }
    }
}