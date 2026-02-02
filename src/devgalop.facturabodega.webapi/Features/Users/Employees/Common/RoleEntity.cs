using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.Common
{
    /// <summary>
    /// Estados permitidos para un rol
    /// </summary>
    public enum RoleStatus
    {
        ACTIVE = 1,
        INACTIVE = 0
    }


    public class RoleEntity
    {
        public Guid Id { get; set; }
        public string Name { get; private set; }
        public RoleStatus Status { get; set; }
        public ICollection<PermissionEntity> Permissions { get; set; }
        
        private RoleEntity()
        {
            Name = string.Empty;
            Permissions = new List<PermissionEntity>();
        }

        public RoleEntity(string name)
        {
            Id = Guid.CreateVersion7();
            Name = name;
            Status = RoleStatus.ACTIVE;
            Permissions = new List<PermissionEntity>();
        }

        public void Deactivate()
        {
            Status = RoleStatus.INACTIVE;
        }
    }


    public class PermissionEntity
    {
        public Guid Id { get; set; }
        public string Name { get; private set; }

        public ICollection<RoleEntity> RolesAssociated { get; set; }

        private PermissionEntity()
        {
            Name = string.Empty;
            RolesAssociated = new List<RoleEntity>();
        }

        public PermissionEntity(string name)
        {
            Id = Guid.CreateVersion7();
            Name = name;
            RolesAssociated = new List<RoleEntity>();
        }
    }
}