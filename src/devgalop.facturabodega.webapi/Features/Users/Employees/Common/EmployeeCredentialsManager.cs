using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Features.Users.Common;
using Microsoft.AspNetCore.Identity;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.Common
{
    
    /// <summary>
    /// Credenciales de un empleado.
    /// </summary>
    /// <param name="Email">Correo del empleado</param>
    /// <param name="Password">Contraseña</param>
    public record EmployeeCredentials(string Email, string Password);

    public class EmployeeCredentialsManager(IPasswordHasher<EmployeeCredentials> passwordHasher) : IPasswordManager<EmployeeCredentials>
    {
        private readonly IPasswordHasher<EmployeeCredentials> _passwordHasher = passwordHasher;

        public string HashPassword(EmployeeCredentials user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyHashedPassword(EmployeeCredentials user, string hashedPassword, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }

    public static class EmployeeCredentialsManagerExtensions
    {
        /// <summary>
        /// Registra la dependencia del administrador de contraseñas para el feature de agregar empleado.
        /// </summary>
        /// <param name="builder">builder de aplicación</param>
        /// <returns></returns>
        public static WebApplicationBuilder RegisterEmployeeCredentialsManager(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IPasswordManager<EmployeeCredentials>, EmployeeCredentialsManager>();
            builder.Services.AddScoped<IPasswordHasher<EmployeeCredentials>, PasswordHasher<EmployeeCredentials>>();
            return builder;
        }
    }
}