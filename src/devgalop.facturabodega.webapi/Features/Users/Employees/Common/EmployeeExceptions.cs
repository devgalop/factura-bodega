using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.Common
{
    public class EmployeeNotFoundException : Exception
    {
        public string Id {get;}
        public EmployeeNotFoundException(string id) 
            : base($"El empleado '{id}' no se encuentra registrado.")
        {
            Id = id;
        }
    }


    public sealed class EmployeeRoleNotFoundException : Exception
    {
        public string RoleName {get;}
        public EmployeeRoleNotFoundException(string roleName) 
            : base($"El rol '{roleName}' no se encuentra registrado.")
        {
            RoleName = roleName;
        }
    }
}