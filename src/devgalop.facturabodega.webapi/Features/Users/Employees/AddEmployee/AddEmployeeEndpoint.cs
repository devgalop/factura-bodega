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
    public record AddEmployeeRequest(string Name,
                                     string Email,
                                     DateTime HiringDate,
                                     string ContractType);
    
    public class AddEmployeeEndpoint : IEndpoint
    {
        public async Task MapEndpoint(IEndpointRouteBuilder app)
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
                    request.HiringDate,
                    Enum.Parse<EmployeeContractType>(request.ContractType)
                ));
                
                return Results.Ok("Empleado agregado exitosamente.");
            });
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
        }

        private bool BeInPast(DateTime date)
        {
            return date <= DateTime.UtcNow;
        }
    }

    
}