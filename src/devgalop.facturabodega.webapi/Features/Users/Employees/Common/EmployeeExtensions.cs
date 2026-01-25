using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Features.Users.Employees.AddEmployee;

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
            builder.RegisterAddEmployeeFeature();
            builder.RegisterAddEmployeePasswordManager();
            return builder;
        }
    }
}