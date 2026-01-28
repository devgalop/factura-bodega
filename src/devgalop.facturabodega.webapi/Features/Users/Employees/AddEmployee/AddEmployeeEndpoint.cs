using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using FluentValidation;
using FluentValidation.Results;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.AddEmployee
{
    /// <summary>
    /// Solicitud para agregar un nuevo empleado.
    /// </summary>
    /// <param name="Name">Nombre del empleado</param>
    /// <param name="Email">Correo del empleado</param>
    /// <param name="Password">Contraseña del empleado</param>
    /// <param name="HiringDate">Fecha de contratación. No puede ser futura</param>
    /// <param name="ContractType">Tipo de contrato. (FULL_TIME, PART_TIME, CONTRACTOR)</param>
    public record AddEmployeeRequest(
        string Name,
        string Email,
        string Password,
        DateTime HiringDate,
        string ContractType
    );
    
    public sealed class AddEmployeeEndpoint : IEndpoint
    {
        /// <summary>
        /// Respuesta al agregar un nuevo empleado.
        /// </summary>
        /// <param name="IsSuccessful">Resultado de la operación</param>
        /// <param name="Message">Mensaje descriptivo</param>
        public record AddEmployeeResponse(bool IsSuccessful, string Message);

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/employees/add", async (
                    AddEmployeeRequest request, 
                    IMediator mediator,
                    IValidator<AddEmployeeRequest> validator) =>
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

                await mediator.SendAsync(new AddEmployeeCommand(
                    request.Name,
                    request.Email,
                    request.Password,
                    request.HiringDate,
                    Enum.Parse<EmployeeContractType>(request.ContractType)
                ));
                
                return Results.Json(new AddEmployeeResponse(true, "Empleado registrado en la base de datos de manera exitosa."), statusCode: 201);
            })
            .WithName("AddEmployee")
            .WithSummary("Agregar Empleado")
            .WithDescription(""" 
                Agrega un nuevo empleado al sistema con la información proporcionada.
                - `Name`: Nombre completo del empleado.
                - `Email`: Correo electrónico del empleado.
                - `Password`: Contraseña para la cuenta del empleado. 
                - `HiringDate`: Fecha de contratación del empleado (no puede ser futura).
                - `ContractType`: Tipo de contrato del empleado (por ejemplo, 'FULL_TIME', 'PART_TIME', 'CONTRACTOR').
            """)
            .Produces<AddEmployeeResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
        }
    }

    internal sealed class AddEmployeeRequestValidator : AbstractValidator<AddEmployeeRequest>
    {
        public AddEmployeeRequestValidator()
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
            
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener almenos 8 carácteres.")
                .MaximumLength(16).WithMessage("La longitud de la contraseña no puede exceder 16 carácteres.")
                .Matches(@"[A-Z]+").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
                .Matches(@"[a-z]+").WithMessage("La contraseña debe contener al menos una letra minúscula.")
                .Matches(@"[0-9]+").WithMessage("La contraseña debe contener al menos un número.")
                .Matches(@"[\!\?\*\.\-]+").WithMessage("La contraseña debe contener al menos un simbolo (!? *.-).");
        }

        private bool BeInPast(DateTime date)
        {
            return date <= DateTime.UtcNow;
        }
    }
}