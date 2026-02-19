using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.Common
{
    public interface ICreateRoleRepository
    {
        /// <summary>
        /// Crea un nuevo rol en la base de datos.
        /// </summary>
        /// <param name="role">Role</param>
        /// <returns></returns>
        Task CreateRole(RoleEntity role);
    }

    public interface IGetRoleRepository
    {
        /// <summary>
        /// Obtiene un rol por su identificador.
        /// </summary>
        /// <param name="id">Identificador del rol</param>
        /// <returns>Rol encontrado</returns>
        Task<RoleEntity?> GetRoleById(string id);
        
        /// <summary>
        /// Obtiene un rol por su nombre.
        /// </summary>
        /// <param name="name">Nombre del rol</param>
        /// <returns>Rol encontrado</returns>
        Task<RoleEntity?> GetRoleByName(string name);

        /// <summary>
        /// Obtiene todos los roles registrados en la base de datos.
        /// </summary>
        /// <returns>Lista de roles</returns>
        Task<List<RoleEntity>> GetAllRoles();
    } 
}