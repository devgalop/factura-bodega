using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.RemoveEmployee
{
    /// <summary>
    /// Solicitud para eliminar un empleado existente.
    /// </summary>
    /// <param name="EmployeeId">Identificador del empleado</param>
    public record RemoveEmployeeRequest(
        string EmployeeId
    ): ICommand;

    /// <summary>
    /// Manejador para procesar la solicitud de eliminación de un empleado.
    /// </summary>
    /// <param name="databaseContext"></param>
    public sealed class RemoveEmployeeHandler (AppDatabaseContext databaseContext)
                : ICommandHandler<RemoveEmployeeRequest>
    {
        public async Task HandleAsync(RemoveEmployeeRequest command)
        {
            var employee = await databaseContext.Employees
                .FirstOrDefaultAsync(e => e.Id.ToString() == command.EmployeeId)
                ?? throw new EmployeeNotFoundException(command.EmployeeId);
            
            employee.Status = EmployeeStatus.INACTIVE;
            databaseContext.Employees.Update(employee);
            await databaseContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Extensiones para el feature de eliminar empleado.
    /// </summary>
    public static class RemoveEmployeeExtensions
    {
        /// <summary>
        /// Registra la dependecia del handler para el feature de eliminar empleado.
        /// </summary>
        /// <param name="builder"></param>
        public static WebApplicationBuilder RegisterRemoveEmployeeFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICommandHandler<RemoveEmployeeRequest>, RemoveEmployeeHandler>();
            return builder;
        }
    }
}