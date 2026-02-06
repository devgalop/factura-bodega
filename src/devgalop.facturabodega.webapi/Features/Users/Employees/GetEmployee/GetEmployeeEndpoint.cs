using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using FluentValidation;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.GetEmployee
{
    /// <summary>
    /// Endpoint para obtener un empleado por su Id.
    /// </summary>
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
                    var response = await mediator.SendAsync<GetEmployeeRequest, GetEmployeeResponse>(request);
                    return Results.Ok(response);
                }
                catch (EmployeeNotFoundException)
                {
                    return Results.NotFound(new { Message = $"Empleado con Id '{employeeId}' no encontrado." });
                }
            })
            .RequireAuthorization("CanListEmployees")
            .WithName("GetEmployeeById")
            .WithSummary("Obtener empleado por Id")
            .WithDescription(""" 
                Obtener la información existente de un empleado en el sistema por el identificador.
                - `employeeId`: Identificador del empleado.
            """)
            .Produces<GetEmployeeResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesValidationProblem();
        }
    }

    /// <summary>
    /// Petición para obtener la lista de empleados.
    /// </summary>
    /// <param name="IncludeInactive">Bandera para indicar si quiere los empleados inactivos, Por defecto es false</param>
    public record GetEmployeesRequest(bool IncludeInactive = false):IQuery;

    /// <summary>
    /// Endpoint para obtener la lista de empleados.
    /// </summary>
    public class GetEmployeesEnpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/employees", async(
                IMediator mediator) =>
            {
                var response = await mediator.SendAsync<GetEmployeesRequest, GetEmployeesResponse>(new GetEmployeesRequest());
                return Results.Ok(response);
            })
            .RequireAuthorization("AdminOnly")
            .WithName("GetEmployees")
            .WithSummary("Obtener empleados existentes")
            .WithDescription(""" 
                Obtener la información existente de los empleado en el sistema.
            """)
            .Produces<GetEmployeesResponse>(StatusCodes.Status200OK);
        }
    }

    /// <summary>
    /// Validador para la petición de obtención de un empleado.
    /// </summary>
    public sealed class GetEmployeeRequestValidator : AbstractValidator<GetEmployeeRequest>
    {
        public GetEmployeeRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("El correo electrónico no puede estar vacío.")
                .MaximumLength(100).WithMessage("El correo electrónico no puede exceder los 100 caracteres.");
        }
    }
}