using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Infrastructure.Persistence.Users.Roles
{
    public sealed class RoleRepository(AppDatabaseContext dbContext): 
        ICreateRoleRepository, 
        IGetRoleRepository
    {
        public async Task CreateRole(RoleEntity role)
        {
            dbContext.Roles.Add(role);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<RoleEntity>> GetAllRoles()
        {
            var roles = await dbContext.Roles.ToListAsync();
            return roles;
        }

        public async Task<RoleEntity?> GetRoleById(string id)
        {
            var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Id.ToString() == id);
            return role;
        }

        public async Task<RoleEntity?> GetRoleByName(string name)
        {
            var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == name);
            return role;
        }
    }

    public static class RoleRepositoryExtensions
    {
        /// <summary>
        /// Registra el repositorio de roles en el contenedor de inyección de dependencias.
        /// </summary>
        /// <param name="builder">Builder de aplicación</param>
        /// <returns>Builder de aplicación</returns>
        public static WebApplicationBuilder AddRoleRepository(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICreateRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IGetRoleRepository, RoleRepository>();
            return builder;
        }
    }
}