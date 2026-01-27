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
        IConfiguration configuration
        ) : IQueryHandler<LoginQuery, LoginResult>
    {
        private readonly AppDatabaseContext _dbContext = dbContext;
        private readonly IPasswordManager<EmployeeCredentials> _passwordManager = passwordManager;
        private readonly JwtOptions _jwtOptions = new JwtOptions(
            configuration.GetValue<string>("Jwt:Issuer") ?? throw new ArgumentNullException("Jwt:Issuer no está configurado apropiadamente en los appsettings."),
            configuration.GetValue<string>("Jwt:Audience") ?? throw new ArgumentNullException("Jwt:Audience no está configurado apropiadamente en los appsettings."),
            configuration.GetValue<int>("Jwt:ExpirationInMinutes"),
            configuration.GetValue<string>("Jwt:SecretKey") ?? throw new ArgumentNullException("Jwt:SecretKey no está configurado apropiadamente en los appsettings.")
        );

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

           
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var tokenCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims =
            [
                new Claim(ClaimTypes.NameIdentifier, employeeFound.Id.ToString()),
                new Claim(ClaimTypes.Email, employeeFound.Email),
                new Claim(ClaimTypes.Role, employeeFound.Role.Name)
            ];
            DateTime expiration = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = tokenCredentials
            };
            var tokenHandler = new JsonWebTokenHandler();
            return new LoginResult(true, tokenHandler.CreateToken(tokenDescriptor), expiration);
            
        }
    }

    public record LoginQuery(string Email, string Password) : IQuery;
    public record LoginResult(bool IsSuccessful, string Token, DateTime Expiration);

    public record JwtOptions(string Issuer, string Audience, int ExpirationInMinutes, string SecretKey);
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