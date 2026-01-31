using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using FluentValidation;
using FluentValidation.Results;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.EditEmployee
{
    /// <summary>
    /// Solicitud para editar un empleado existente.
    /// </summary>
    /// <param name="Email">Email registrado</param>
    /// <param name="Name">Nombre empleado</param>
    /// <param name="ContractType">Tipo de contrato</param>
    /// <param name="HiringDate">Fecha de contratación. No puede ser futura</param>
    /// <param name="Role">Nombre del rol</param>
    /// <param name="IsActive">Estado actual del empleado</param>
    public record EditEmployeeRequest(
        string Email, 
        string Name, 
        string ContractType, 
        DateTime HiringDate, 
        string Role, 
        bool IsActive 
    );

    /// <summary>
    /// Respuesta al editar un empleado existente.
    /// </summary>
    /// <param name="IsSuccessful">Resultado del proceso</param>
    /// <param name="Message">Detalle del proceso</param>
    public record EditEmployeeResponse(bool IsSuccessful, string Message);

    public sealed class EditEmployeeEndoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/employees/edit", async (
                    EditEmployeeRequest request, 
                    IMediator mediator,
                    IValidator<EditEmployeeRequest> validator) =>
            {
                validator.ValidateAndThrow(request);

                if(!Enum.TryParse(
                    request.ContractType,
                    ignoreCase: true,
                    out EmployeeContractType contractType)) throw new ValidationException(
                                                                [
                                                                    new ValidationFailure(
                                                                        nameof(request.ContractType),
                                                                        "El tipo de contrato proporcionado no es válido.")
                                                                ]);

                await mediator.SendAsync(new EditEmployeeCommand(
                    request.Email,
                    request.Name,
                    contractType,
                    request.HiringDate,
                    request.Role,
                    request.IsActive
                ));
                
                return Results.Json(new EditEmployeeResponse(true, "Empleado editado correctamente."), statusCode: 202);
            })
            .RequireAuthorization("AdminOnly")
            .WithName("EditEmployee")
            .WithSummary("Editar información de Empleado")
            .WithDescription(""" 
                Editar la información existente de un empleado en el sistema.
                - `Email`: Correo electrónico del empleado.
                - `Name`: Nombre completo del empleado.
                - `ContractType`: Tipo de contrato del empleado (por ejemplo, 'FULL_TIME', 'PART_TIME', 'CONTRACTOR').
                - `HiringDate`: Fecha de contratación del empleado (no puede ser futura).
                - `Role`: Nombre del rol asignado al empleado.
                - `IsActive`: Estado actual del empleado (true para activo, false para inactivo).
            """)
            .Produces<EditEmployeeResponse>(StatusCodes.Status202Accepted)
            .ProducesValidationProblem();
        }
    }

    public sealed class EditEmployeeValidator : AbstractValidator<EditEmployeeRequest>
    {
        public EditEmployeeValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El correo electrónico no es válido.")
                .MaximumLength(100).WithMessage("El correo electrónico no puede exceder los 100 caracteres.");

            RuleFor(x => x.HiringDate)
                .Must(BeInPast).WithMessage("La fecha de contratación no puede ser futura.");

            RuleFor(x => x.ContractType)
                .NotEmpty().WithMessage("El tipo de contrato es obligatorio.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("El rol del empleado es obligatorio.");
        }
        private bool BeInPast(DateTime date)
        {
            return date <= DateTime.UtcNow;
        }
    }
}