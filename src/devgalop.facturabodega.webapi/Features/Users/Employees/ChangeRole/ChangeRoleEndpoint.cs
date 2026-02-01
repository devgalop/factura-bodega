using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using FluentValidation;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.ChangeRole
{
    /// <summary>
    /// Solicitud para cambiar el rol de un empleado
    /// </summary>
    /// <param name="EmployeeId">Identificador del empleado</param>
    /// <param name="RoleName">Nombre empleado</param>
    public record ChangeRoleRequest(string EmployeeId, string RoleName);

    /// <summary>
    /// Respuesta para cambiar el rol de un empleado
    /// </summary>
    /// <param name="Message">Mensaje descriptivo</param>
    public record ChangeRoleResponse(string Message);

    /// <summary>
    /// Endpoint para cambiar el rol de un empleado
    /// </summary>
    public sealed class ChangeRoleEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/employees/change-role", async (
                    ChangeRoleRequest request, 
                    IMediator mediator,
                    IValidator<ChangeRoleRequest> validator
            ) =>
            {
                validator.ValidateAndThrow(request);
                await mediator.SendAsync(new ChangeRoleCommand(
                    request.EmployeeId,
                    request.RoleName
                ));
                return Results.Json(new ChangeRoleResponse("El rol del empleado ha sido cambiado exitosamente."), statusCode: 202);
            })
            .RequireAuthorization("AdminOnly")
            .WithName("ChangeRole")
            .WithSummary("Editar el rol del Empleado")
            .WithDescription(""" 
                Editar el rol de un empleado en el sistema.
                - `EmployeeId`: Identificador del empleado.
                - `RoleName`: Nombre del rol.
            """)
            .Produces<ChangeRoleResponse>(StatusCodes.Status202Accepted)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();
        }
    }

    public sealed class ChangeRoleValidator : AbstractValidator<ChangeRoleRequest>
    {
        public ChangeRoleValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("El identificador del empleado es obligatorio.")
                .MaximumLength(100).WithMessage("El identificador del empleado no puede exceder los 100 caracteres.");

            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("El nombre del rol es obligatorio.");
        }
    }
}