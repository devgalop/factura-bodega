using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Users.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.Login
{
    public class LoginHandler(
        AppDatabaseContext dbContext, 
        IPasswordManager<EmployeeCredentials> passwordManager,
        TokenFactoryService tokenFactoryService
        ) : IQueryHandler<LoginQuery, LoginResult>
    {
        private readonly AppDatabaseContext _dbContext = dbContext;
        private readonly IPasswordManager<EmployeeCredentials> _passwordManager = passwordManager;
        private readonly TokenFactoryService _tokenFactoryService = tokenFactoryService;

        public async Task<LoginResult> HandleAsync(LoginQuery query)
        {
            var employeeFound = _dbContext.Employees
                                    .Include(e => e.Role)
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

            List<Claim> claims =
            [
                new Claim(ClaimTypes.NameIdentifier, employeeFound.Id.ToString()),
                new Claim(ClaimTypes.Email, employeeFound.Email),
                new Claim(ClaimTypes.Role, employeeFound.Role.Name)
            ];

            var tokenResult = _tokenFactoryService.CreateToken(claims);

            var refreshTokenResult = _tokenFactoryService.GenerateRefreshToken();

            var refreshToken = new EmployeeRefreshTokenEntity(
                refreshTokenResult.Token,
                refreshTokenResult.Expiration,
                employeeFound);
            _dbContext.RefreshTokens.Add(refreshToken);
            await _dbContext.SaveChangesAsync();
            
            return new LoginResult(tokenResult.Token, refreshTokenResult.Token);
            
        }
    }

    /// <summary>
    /// Consulta para el inicio de sesión de empleado.
    /// </summary>
    /// <param name="Email">Correo del empleado</param>
    /// <param name="Password">Contraseña del empleado</param>
    public record LoginQuery(string Email, string Password) : IQuery;

    /// <summary>
    /// Resultado del inicio de sesión de empleado.
    /// </summary>
    /// <param name="AccessToken">Token de acceso</param>
    /// <param name="RefreshToken">Token para refrescar</param>
    public record LoginResult(string AccessToken, string RefreshToken);

    /// <summary>
    /// Extensiones para el feature de inicio de sesión de empleado.
    /// </summary>    
    public static class LoginExtensions
    {
        /// <summary>
        /// Registra la dependecia del handler para el feature de inicio de sesión de empleado.
        /// </summary>
        /// <param name="builder">builder de aplicación</param>
        /// <returns></returns>
        public static WebApplicationBuilder RegisterLoginFeature(this WebApplicationBuilder builder)
        {
            var optionsJWT = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? throw new ArgumentNullException("Jwt no está configurado apropiadamente en los appsettings.");
            builder.Services.AddSingleton(optionsJWT);
            builder.Services.AddScoped<IQueryHandler<LoginQuery, LoginResult>, LoginHandler>();
            return builder;
        }
    }
}