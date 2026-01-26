using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Users.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using FluentValidation;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.Login
{
    public class LoginHandler(AppDatabaseContext dbContext, IPasswordManager<EmployeeCredentials> passwordManager) : IQueryHandler<LoginQuery, LoginResult>
    {
        private readonly AppDatabaseContext _dbContext = dbContext;
        private readonly IPasswordManager<EmployeeCredentials> _passwordManager = passwordManager;

        public async Task<LoginResult> HandleAsync(LoginQuery query)
        {
            var employeeFound = _dbContext.Employees
                                    .Where(e => e.Email == query.Email)
                                    .FirstOrDefault() 
                                    ?? throw new ValidationException(
                                        [
                                            new FluentValidation.Results.ValidationFailure(
                                                nameof(query.Email),
                                                "El correo electrónico no se encuentra registrado.")
                                        ]);

            EmployeeCredentials credentials = new(query.Email, query.Password);
            
            bool isPasswordValid = _passwordManager.VerifyHashedPassword(
                credentials,  
                employeeFound.PasswordHashed,
                query.Password);
            
            if(!isPasswordValid) throw new ValidationException(
                                        [
                                            new FluentValidation.Results.ValidationFailure(
                                                nameof(query.Password),
                                                "La contraseña proporcionada es incorrecta.")
                                        ]);

            //TODO: Generar y retornar un token JWT al usuario autenticado.
            return new LoginResult(true, "", DateTime.UtcNow);
        }
    }

    public record LoginQuery(string Email, string Password) : IQuery;
    public record LoginResult(bool IsSuccessful, string Token, DateTime Expiration);

    public static class LoginExtensions
    {
        /// <summary>
        /// Registra la dependecia del handler para el feature de inicio de sesión de empleado.
        /// </summary>
        /// <param name="builder">builder de aplicación</param>
        /// <returns></returns>
        public static WebApplicationBuilder RegisterLoginFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IQueryHandler<LoginQuery, LoginResult>, LoginHandler>();
            return builder;
        }
    }
}