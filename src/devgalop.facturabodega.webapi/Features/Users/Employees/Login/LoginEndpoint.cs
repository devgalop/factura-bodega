using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using FluentValidation;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.Login
{
    /// <summary>
    /// Solicitud de inicio de sesión de empleado.
    /// </summary>
    /// <param name="Email">Correo del empleado</param>
    /// <param name="Password">Contraseña</param>
    public record LoginRequest(
        string Email,
        string Password
    );


    public sealed class LoginEndpoint : IEndpoint
    {
        public async Task MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/employees/login", async (
                    LoginRequest request, 
                    IMediator mediator,
                    IValidator<LoginRequest> validator) =>
            {
                validator.ValidateAndThrow(request);

                LoginResult result = await mediator.SendAsync<LoginQuery, LoginResult>(new LoginQuery(
                    request.Email,
                    request.Password
                ));
                
                return Results.Ok(result);
            })
            .WithName("LoginEmployee")
            .WithSummary("Inicio de sesión de empleado")
            .WithDescription(""" 
                Inicia sesión en la cuenta de un empleado existente utilizando su correo electrónico y contraseña.
                - `Email`: Correo electrónico del empleado.
                - `Password`: Contraseña para la cuenta del empleado.
            """)
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem();
        }
    }

    internal sealed class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El correo electrónico no es válido.");

            RuleFor(x => x.Password)
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