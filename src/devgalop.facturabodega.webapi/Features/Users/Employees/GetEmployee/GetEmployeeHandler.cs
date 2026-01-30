using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.GetEmployee
{
    /// <summary>
    /// Petición para obtener un empleado por su correo electrónico.
    /// </summary>
    /// <param name="Id">Identificador del empleado</param>
    public record GetEmployeeRequest(string Id):IQuery;

    /// <summary>
    /// Respuesta con los detalles del empleado.
    /// </summary>
    /// <param name="Id">Identificador único del empleado</param>
    /// <param name="Name">Nombre del empleado</param>
    /// <param name="Email">Correo del empleado</param>
    /// <param name="HiringDate">Fecha de contratación</param>
    /// <param name="ContractType">Tipo de contrato</param>
    /// <param name="RoleName">Nombre del role</param>
    /// <param name="IsActive">Estado del empleado</param>
    public record GetEmployeeResponse(
        Guid Id, 
        string Name, 
        string Email, 
        DateTime HiringDate, 
        string ContractType, 
        string RoleName, 
        bool IsActive
    );

    /// <summary>
    /// Manejador para procesar la solicitud de obtención de un empleado.
    /// </summary>
    /// <param name="dbContext"></param>
    public class GetEmployeeHandler(AppDatabaseContext dbContext) 
                : IQueryHandler<GetEmployeeRequest, GetEmployeeResponse>
    {
        public async Task<GetEmployeeResponse> HandleAsync(GetEmployeeRequest query)
        {
            var employeeFound = dbContext.Employees
                .Where(e => e.Id.ToString() == query.Id)
                .Select(e => new GetEmployeeResponse(
                    e.Id,
                    e.Name,
                    e.Email,
                    e.HiringDate,
                    e.ContractType.ToString(),
                    e.Role.Name,
                    e.Status == EmployeeStatus.ACTIVE
                ))
                .FirstOrDefault() ?? throw new EmployeeNotFoundException(query.Id);
            
            return employeeFound;
        }
    }
}