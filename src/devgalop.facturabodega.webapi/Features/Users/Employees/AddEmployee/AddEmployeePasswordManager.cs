using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Features.Users.Common;
using Microsoft.AspNetCore.Identity;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.AddEmployee
{
    public class AddEmployeePasswordManager(IPasswordHasher<AddEmployeeCommand> passwordHasher) : IPasswordManager<AddEmployeeCommand>
    {
        private readonly IPasswordHasher<AddEmployeeCommand> _passwordHasher = passwordHasher;

        public string HashPassword(AddEmployeeCommand user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyHashedPassword(AddEmployeeCommand user, string hashedPassword, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }

    public static class AddEmployeePasswordManagerExtensions
    {
        /// <summary>
        /// Registra la dependencia del administrador de contraseñas para el feature de agregar empleado.
        /// </summary>
        /// <param name="builder">builder de aplicación</param>
        /// <returns></returns>
        public static WebApplicationBuilder RegisterAddEmployeePasswordManager(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IPasswordManager<AddEmployeeCommand>, AddEmployeePasswordManager>();
            builder.Services.AddScoped<IPasswordHasher<AddEmployeeCommand>, PasswordHasher<AddEmployeeCommand>>();
            return builder;
        }
    }
}