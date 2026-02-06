using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Features.Users.Common
{
    public class RolePermissionsProvider(IServiceProvider serviceProvider)
    {
        private Dictionary<string, List<string>> _rolePermissions =[];

        public bool HasPermission(string role, string permission)
        {
            if (_rolePermissions.TryGetValue(role, out var permissions))
            {
                return permissions.Contains(permission);
            }
            return false;
        }

        public void LoadPermissions()
        {
            var dbContext = serviceProvider.GetRequiredService<AppDatabaseContext>();
            _rolePermissions = dbContext.Roles
                .Include(r => r.Permissions)
                .ToDictionary(
                    r => r.Name,
                    r => r.Permissions.Select(p => p.Name).ToList()
                );
        }
    }
}