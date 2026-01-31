using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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
    /// Respuesta con la información mínima del empleado.
    /// </summary>
    /// <param name="Id">Identificacion del empleado</param>
    /// <param name="Name">Nombre del empleado</param>
    /// <param name="IsActive">Esta activo en el sistema</param>
    public record GetEmployeesMinimunInfoResponse(
        Guid Id,
        string Name,
        bool IsActive
    );

    /// <summary>
    /// Respuesta con la lista de empleados.
    /// </summary>
    /// <param name="Employees">Lista de empleados</param>
    public record GetEmployeesResponse(
        List<GetEmployeesMinimunInfoResponse> Employees
    );

    /// <summary>
    /// Manejador para procesar la solicitud de obtención de un empleado.
    /// </summary>
    /// <param name="dbContext"></param>
    public sealed class GetEmployeeHandler(AppDatabaseContext dbContext) 
                : IQueryHandler<GetEmployeeRequest, GetEmployeeResponse>
    {
        public async Task<GetEmployeeResponse> HandleAsync(GetEmployeeRequest query)
        {
            var employeeFound = await dbContext.Employees
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
                .FirstOrDefaultAsync() ?? throw new EmployeeNotFoundException(query.Id);
            
            return employeeFound;
        }
    }

    /// <summary>
    /// Manejador para procesar la solicitud de obtención de la lista de empleados.
    /// </summary>
    /// <param name="dbContext"></param>
    public sealed class GetEmployeesHandler(AppDatabaseContext dbContext)
        : IQueryHandler<GetEmployeesRequest, GetEmployeesResponse>
    {
        public async Task<GetEmployeesResponse> HandleAsync(GetEmployeesRequest query)
        {
            var employeesQuery = dbContext.Employees.AsQueryable();

            if (!query.IncludeInactive)
            {
                employeesQuery = employeesQuery.Where(e => e.Status == EmployeeStatus.ACTIVE);
            }

            var employeesList = employeesQuery
                .Select(e => new GetEmployeesMinimunInfoResponse(
                    e.Id,
                    e.Name,
                    e.Status == EmployeeStatus.ACTIVE
                ))
                .ToList();

            return new GetEmployeesResponse(employeesList);
        }
    }


    public static class GetEmployeeExtensions
    {
        /// <summary>
        /// Registra los servicios de los features de obtención de empleados.
        /// </summary>
        /// <param name="builder">builder de aplicación</param>
        public static WebApplicationBuilder RegisterGetEmployeeFeatures(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IQueryHandler<GetEmployeeRequest, GetEmployeeResponse>, GetEmployeeHandler>()
                            .AddScoped<IQueryHandler<GetEmployeesRequest, GetEmployeesResponse>, GetEmployeesHandler>();
            return builder;
        }
    }

}