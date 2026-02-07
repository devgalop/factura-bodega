using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using FluentValidation;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.RecoveryPassword
{
    /// <summary>
    /// Peticion para recuperar la contraseña de un usuario, se le enviará un correo con un token para restablecer su contraseña.
    /// </summary>
    /// <param name="Email">Correo del empleado</param>
    public record RecoveryPasswordRequest(string Email);

    /// <summary>
    /// Respuesta de la petición de recuperación de contraseña, se le enviará un mensaje indicando que se ha enviado el correo para restablecer la contraseña.
    /// </summary>
    /// <param name="Message">Mensaje descriptivo</param>
    public record RecoveryPasswordResponse(string Message);

    public sealed class RecoveryPasswordEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/employees/recovery-password", async (
                RecoveryPasswordRequest request,
                IMediator mediator,
                IValidator<RecoveryPasswordRequest> validator
            ) =>
            {
                validator.ValidateAndThrow(request);
                await mediator.SendAsync(new RecoveryPasswordCommand(request.Email));
                return Results.Ok(new RecoveryPasswordResponse("Si el correo electrónico proporcionado está registrado, se ha enviado un correo con las instrucciones para restablecer la contraseña."));
            })
            .AllowAnonymous()
            .WithName("RecoveryPasswordEmployee")
            .WithSummary("Recuperación de contraseña de empleado")
            .WithDescription(""" 
                Recuperar contraseña de empleado registrado.
                - `Email`: Correo electronico del empleado.
            """)
            .Produces<RecoveryPasswordResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem();
        }
    }

    public sealed class RecoveryPasswordRequestValidator : AbstractValidator<RecoveryPasswordRequest>
    {
        public RecoveryPasswordRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El correo electrónico no es válido.")
                .MaximumLength(100).WithMessage("El correo electrónico no puede exceder los 100 caracteres.");
        }
    }
}