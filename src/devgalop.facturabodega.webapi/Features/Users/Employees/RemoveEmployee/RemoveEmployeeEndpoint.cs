using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using Microsoft.AspNetCore.Mvc;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.RemoveEmployee
{
    /// <summary>
    /// Respuesta de la operación de eliminación de un empleado.
    /// </summary>
    /// <param name="IsSuccessful">Resultado de la operacion</param>
    /// <param name="Message">Descripción del resultado</param>
    public record RemoveEmployeeResponse(bool IsSuccessful, string Message);

    /// <summary>
    /// Endpoint para eliminar un empleado existente.
    /// </summary>
    public class RemoveEmployeeEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("/employees/remove", async (
                    [FromQuery] string employeeId,
                    IMediator mediator
            ) =>
            {
                if(string.IsNullOrEmpty(employeeId))
                {
                    return Results.BadRequest(new RemoveEmployeeResponse(
                        false, 
                        "El identificador del empleado es obligatorio."
                    ));
                }
                await mediator.SendAsync(new RemoveEmployeeRequest(employeeId));
                return Results.Ok(new RemoveEmployeeResponse(
                    true, 
                    "El empleado ha sido desactivado correctamente."
                ));
            })
            .RequireAuthorization("CanRemoveEmployee")
            .WithName("RemoveEmployee")
            .WithSummary("Dar de baja el empleado")
            .WithDescription(""" 
                Dar de baja un empleado en el sistema.
                - `EmployeeId`: Identificador del empleado.
            """)
            .Produces<RemoveEmployeeResponse>(StatusCodes.Status200OK)
            .Produces<RemoveEmployeeResponse>(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden);
        }
    }
}