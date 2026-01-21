using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

    public static class EndpointExtensions
    {
        /// <summary>
        /// Registra las dependencias de todos los endpoints en el contenedor de servicios
        /// </summary>
        /// <param name="builder">builder de aplicación</param>
        /// <returns></returns>
        public static WebApplicationBuilder AddEndpoints(this WebApplicationBuilder builder)
        {
            // Registrar todos los tipos que implementen IEndpoint en el contenedor de servicios
            var serviceDescriptors = Assembly.GetExecutingAssembly()
                .DefinedTypes
                .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                               typeof(IEndpoint).IsAssignableFrom(type))
                .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
                .ToArray();

            builder.Services.TryAddEnumerable(serviceDescriptors);
            return builder;
        }

        /// <summary>
        /// Mapea todos los endpoints registrados en la aplicación web
        /// </summary>
        /// <param name="app">aplicaión web</param>
        /// <returns></returns>
        public static WebApplication MapEndpoints(this WebApplication app)
        {
            // Obtener todos los servicios registrados que implementen IEndpoint
            var endpoints = app.Services.GetServices<IEndpoint>();

            // Mapear cada endpoint en la aplicación web
            foreach (var endpoint in endpoints)
            {
                endpoint.MapEndpoint(app);
            }

            return app;
        }
    }
}