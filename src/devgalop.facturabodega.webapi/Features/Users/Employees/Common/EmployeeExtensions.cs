using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Features.Users.Employees.AddEmployee;
using devgalop.facturabodega.webapi.Features.Users.Employees.ChangePassword;
using devgalop.facturabodega.webapi.Features.Users.Employees.ChangeRole;
using devgalop.facturabodega.webapi.Features.Users.Employees.EditEmployee;
using devgalop.facturabodega.webapi.Features.Users.Employees.GetEmployee;
using devgalop.facturabodega.webapi.Features.Users.Employees.Login;
using devgalop.facturabodega.webapi.Features.Users.Employees.RemoveEmployee;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.Common
{
    public static class EmployeeExtensions
    {
        /// <summary>
        /// Registra los servicios de los features de empleados.
        /// </summary>
        /// <param name="builder">builder de aplicación</param>
        public static WebApplicationBuilder RegisterEmployeeFeatures(this WebApplicationBuilder builder)
        {
            builder.RegisterAddEmployeeFeature()
                    .RegisterEditEmployeeFeature()
                    .RegisterGetEmployeeFeatures()
                    .RegisterRemoveEmployeeFeature()
                    .RegisterChangeRoleFeature()
                    .RegisterEmployeeCredentialsManager()
                    .RegisterChangePasswordFeature()
                    .RegisterLoginFeature();
                    
            return builder;
        }
    }
}