using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;

namespace devgalop.facturabodega.webapi.Middlewares
{
    public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public ValueTask<bool> TryHandleAsync(HttpContext httpContext, 
                                                Exception exception, 
                                                CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Ocurrió una excepción no controlada: {Message}", exception.Message);
            return problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new()
                {
                    Title = "Error interno del servidor",
                    Detail = $"Ha ocurrido un error inesperado mientras se procesaba la solicitud. Por favor, intente nuevamente más tarde. {exception.Message}",
                    Status = StatusCodes.Status500InternalServerError
                }
            });
        }
    }
}