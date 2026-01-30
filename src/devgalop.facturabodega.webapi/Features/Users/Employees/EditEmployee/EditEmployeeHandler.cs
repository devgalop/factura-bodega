using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.EditEmployee
{
    /// <summary>
    /// Comando para editar un empleado existente.
    /// </summary>
    /// <param name="Email">Email del empleado</param>
    /// <param name="Name">Nombre del empleado</param>
    /// <param name="ContractType">Tipo de contrato</param>
    /// <param name="HiringDate">Fecha de contratación</param>
    /// <param name="Role">Nombre del rol</param>
    /// <param name="IsActive">Estado del empleado</param>
    public record EditEmployeeCommand(
        string Email, 
        string Name, 
        EmployeeContractType ContractType, 
        DateTime HiringDate, 
        string Role, 
        bool IsActive 
    ) : ICommand;

    public class EditEmployeeHandler(AppDatabaseContext dbContext) : ICommandHandler<EditEmployeeCommand>
    {
        public async Task HandleAsync(EditEmployeeCommand command)
        {
            var employeeFound = dbContext.Employees
                                    .Include(e => e.Role)
                                    .Where(e => e.Email == command.Email)
                                    .FirstOrDefault() ?? throw new EmployeeNotFoundException(command.Email);
            
            if(command.Role != employeeFound.Role.Name)
            {
                var newRole = dbContext.Roles
                                .Where(r => r.Name == command.Role)
                                .FirstOrDefault() ?? throw new EmployeeRoleNotFoundException(command.Role);
                
                employeeFound.Role = newRole;
            }
            employeeFound.Name = command.Name;
            employeeFound.ContractType = command.ContractType;
            employeeFound.HiringDate = command.HiringDate;
            employeeFound.Status = command.IsActive ? EmployeeStatus.ACTIVE : EmployeeStatus.INACTIVE;

            dbContext.Employees.Update(employeeFound);
            await dbContext.SaveChangesAsync();
        }
    }

    public static class EditEmployeeExtensions
    {
        /// <summary>
        /// Registra la dependecia del handler para el feature de editar empleado.
        /// </summary>
        /// <param name="builder">builder de aplicación</param>
        /// <returns></returns>
        public static WebApplicationBuilder RegisterEditEmployeeFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICommandHandler<EditEmployeeCommand>, EditEmployeeHandler>();
            return builder;
        }
    }
}