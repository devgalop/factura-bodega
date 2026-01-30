using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using FluentValidation;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.GetEmployee
{

    public class GetEmployeeEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/employees/{employeeId}", async(
                string employeeId, 
                IMediator mediator,
                IValidator<GetEmployeeRequest> validator)=>
            {
                GetEmployeeRequest request = new(employeeId);
                validator.ValidateAndThrow(request);
                try
                {
                    var response = await mediator.SendAsync<GetEmployeeRequest, GetEmployeeHandler>(request);
                    return Results.Ok(response);
                }
                catch (EmployeeNotFoundException)
                {
                    return Results.NotFound(new { Message = $"Empleado con Id '{employeeId}' no encontrado." });
                }
            });
        }
    }

    internal sealed class GetEmployeeRequestValidator : AbstractValidator<GetEmployeeRequest>
    {
        public GetEmployeeRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El correo electrónico no puede estar vacío.")
                .MaximumLength(100).WithMessage("El correo electrónico no puede exceder los 100 caracteres.");
        }
    }
}