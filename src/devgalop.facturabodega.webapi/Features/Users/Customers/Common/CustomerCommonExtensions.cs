using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Features.Users.Customers.AddCustomer;

namespace devgalop.facturabodega.webapi.Features.Users.Customers.Common
{
    /// <summary>
    /// Dependencias comunes para features de clientes.
    /// </summary>
    public static class CustomerCommonExtensions
    {
        /// <summary>
        /// Registra los features relacionados con clientes.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder RegisterCustomerFeatures(this WebApplicationBuilder builder)
        {
            builder.RegisterAddCustomerFeature();
            return builder;
        }
    }
}