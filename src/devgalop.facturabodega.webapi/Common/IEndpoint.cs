using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devgalop.facturabodega.webapi.Common
{
    public interface IEndpoint
    {
        /// <summary>
        /// Define el mapeo de endpoints en la aplicación web
        /// </summary>
        /// <param name="app">builder de aplicación</param>
        /// <returns></returns>
        Task MapEndpoint(IEndpointRouteBuilder app);
    }
}