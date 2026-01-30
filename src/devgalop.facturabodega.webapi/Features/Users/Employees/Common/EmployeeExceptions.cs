using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.Common
{
    public class EmployeeNotFoundException : Exception
    {
        public string Email {get;}
        public EmployeeNotFoundException(string email) 
            : base($"El empleado con el email '{email}' no se encuentra registrado.")
        {
            Email = email;
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