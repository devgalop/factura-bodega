using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.ChangePassword
{
    /// <summary>
    /// Peticion para cambiar la contraseña de un empleado existente.
    /// </summary>
    /// <param name="CurrentPassword">Contraseña actual</param>
    /// <param name="NewPassword">Contraseña nueva</param>
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

    /// <summary>
    /// Respuesta al cambiar la contraseña de un empleado existente.
    /// </summary>
    /// <param name="IsSuccessful">Resultado de la operación</param>
    /// <param name="Message">Mensaje descriptivo</param>
    public record ChangePasswordResponse(bool IsSuccessful, string Message);

    public sealed class ChangePasswordEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/employees/change-password", async (
                [FromQuery] string userId,
                ChangePasswordRequest request,
                IMediator mediator,
                IValidator<ChangePasswordRequest> validator
            ) =>
            {
                validator.ValidateAndThrow(request);
                if(string.IsNullOrEmpty(userId)) throw new ValidationException(
                                        [
                                            new ValidationFailure(
                                                "userId",
                                                "El identificador del usuario es obligatorio.")
                                        ]);
                if(request.CurrentPassword == request.NewPassword) throw new ValidationException(
                                        [
                                            new ValidationFailure(
                                                nameof(request.NewPassword),
                                                "La contraseña nueva no puede ser igual a la contraseña actual.")
                                        ]);
                await mediator.SendAsync(new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword));

                return Results.Ok(new ChangePasswordResponse(true, "Contraseña cambiada exitosamente."));
            })
            .RequireAuthorization("CanRecoveryPassword")
            .WithName("ChangePassword")
            .WithSummary("Cambiar Contraseña Empleado")
            .WithDescription(""" 
                Cambiar la contraseña de un empleado existente
                - 'userId' es el identificador del usuario (obtenido al listar los empleados)
                - 'CurrentPassword' es la contraseña actual del empleado
                - 'NewPassword' es la nueva contraseña que se desea establecer para el empleado
            """)
            .Produces<ChangePasswordResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesValidationProblem();
        }
    }

    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener almenos 8 carácteres.")
                .MaximumLength(16).WithMessage("La longitud de la contraseña no puede exceder 16 carácteres.")
                .Matches(@"[A-Z]+").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
                .Matches(@"[a-z]+").WithMessage("La contraseña debe contener al menos una letra minúscula.")
                .Matches(@"[0-9]+").WithMessage("La contraseña debe contener al menos un número.")
                .Matches(@"[\!\?\*\.\-]+").WithMessage("La contraseña debe contener al menos un simbolo (!? *.-).");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener almenos 8 carácteres.")
                .MaximumLength(16).WithMessage("La longitud de la contraseña no puede exceder 16 carácteres.")
                .Matches(@"[A-Z]+").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
                .Matches(@"[a-z]+").WithMessage("La contraseña debe contener al menos una letra minúscula.")
                .Matches(@"[0-9]+").WithMessage("La contraseña debe contener al menos un número.")
                .Matches(@"[\!\?\*\.\-]+").WithMessage("La contraseña debe contener al menos un simbolo (!? *.-).");
        }
    }
}