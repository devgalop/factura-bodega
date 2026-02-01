using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.ChangeRole
{
    /// <summary>
    /// Comando para cambiar el rol de un empleado
    /// </summary>
    /// <param name="EmployeeId">Identificador del empleado</param>
    /// <param name="RoleName">Rol del empleado</param>
    public record ChangeRoleCommand(string EmployeeId, string RoleName): ICommand;

    /// <summary>
    /// Manejador para cambiar el rol de un empleado
    /// </summary>
    /// <param name="dbContext"></param>
    public sealed class ChangeRoleHandler(AppDatabaseContext dbContext) 
                : ICommandHandler<ChangeRoleCommand>
    {

        public async Task HandleAsync(ChangeRoleCommand command)
        {
            var employeeFound = await dbContext.Employees
                                                .Include(e => e.Role)
                                                .FirstOrDefaultAsync(e => e.Id.ToString() == command.EmployeeId) 
                                                ?? throw new EmployeeNotFoundException(command.EmployeeId);
            var roleFound = await dbContext.Roles
                                            .FirstOrDefaultAsync(r => r.Name == command.RoleName)
                                            ?? throw new EmployeeRoleNotFoundException(command.RoleName);
            employeeFound.Role = roleFound;
            dbContext.Employees.Update(employeeFound);
            await dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Extensiones para registrar el manejador de cambiar rol de empleado
    /// </summary>
    public static class ChangeRoleExtensions
    {
        /// <summary>
        /// Registra el manejador de cambiar rol de empleado
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder RegisterChangeRoleFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICommandHandler<ChangeRoleCommand>, ChangeRoleHandler>();
            return builder;
        }
    }
}