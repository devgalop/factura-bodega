using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.Common
{
    public interface ICreateEmployeeRepository
    {
        /// <summary>
        /// Crea un nuevo empleado en la base de datos.
        /// </summary>
        /// <param name="employee">Entidad</param>
        /// <returns></returns>
        Task CreateEmployee(EmployeeEntity employee);
    }

    public interface IGetEmployeeRepository
    {
        /// <summary>
        /// Obtiene un empleado por su identificador.
        /// </summary>
        /// <param name="id">Identificador del empleado</param>
        /// <returns>Entidad</returns>
        Task<EmployeeEntity?> GetEmployeeById(string id);
        /// <summary>
        /// Obtiene un empleado por su correo electrónico.
        /// </summary>
        /// <param name="email">Correo electrónico</param>
        /// <returns>Entidad</returns>
        Task<EmployeeEntity?> GetEmployeeByEmail(string email);
        /// <summary>
        /// Obtiene todos los empleados registrados en la base de datos.
        /// </summary>
        /// <returns>Lista de empleados</returns>
        Task<List<EmployeeEntity>> GetAllEmployees();

        /// <summary>
        /// Obtiene una lista de empleados filtrados por su estado (activo o inactivo).
        /// </summary>
        /// <param name="status">Estado del empleado</param>
        /// <returns>Lista de empleados</returns>
        Task<List<EmployeeEntity>> GetEmployeesByStatus(EmployeeStatus status);

        /// <summary>
        /// Obtiene una lista de empleados activos registrados en la base de datos.
        /// </summary>
        /// <returns>Lista de empleados</returns>
        Task<List<EmployeeEntity>> GetActiveEmployees();

        /// <summary>
        /// Obtiene una lista de empleados inactivos registrados en la base de datos.
        /// </summary>
        /// <returns>Lista de empleados</returns>
        Task<List<EmployeeEntity>> GetInactiveEmployees();
    }

    public interface IEditEmployeeRepository
    {
        /// <summary>
        /// Edita la información de un empleado existente en la base de datos.
        /// </summary>
        /// <param name="employee">Entidad</param>
        /// <returns></returns>
        Task EditEmployee(EmployeeEntity employee);
    }

    public interface IRemoveEmployeeRepository
    {
        /// <summary>
        /// Elimina un empleado existente en la base de datos, cambiando su estado a inactivo.
        /// </summary>
        /// <param name="employee">Entidad</param>
        /// <returns></returns>
        Task RemoveEmployee(EmployeeEntity employee);
    }
}