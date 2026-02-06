using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace devgalop.facturabodega.webapi.Features.Users.Common
{
    /// <summary>
    /// Opciones de configuración para JWT.
    /// </summary>
    /// <param name="Issuer">Issuer configurado</param>
    /// <param name="Audience">Audiencia configurada</param>
    /// <param name="ExpirationInMinutes">Vigencia del token</param>
    /// <param name="SecretKey">Semilla para token</param>
    public record JwtOptions(string Issuer, string Audience, int ExpirationInMinutes, string SecretKey);

    /// <summary>
    /// Resultado de la creación de un token.
    /// </summary>
    /// <param name="IsSuccessful">Resultado de operación</param>
    /// <param name="Token">JWT Token generado</param>
    /// <param name="Expiration">Fecha de expiración</param>
    public record TokenResult(bool IsSuccessful, string Token, DateTime Expiration);

    /// <summary>
    /// Servicio para la creación de tokens JWT.
    /// </summary>
    /// <param name="jwtOptions">Opciones de configuracion JWT</param>
    public sealed class TokenFactoryService(JwtOptions jwtOptions)
    {
        
        /// <summary>
        /// Crea un token JWT con los claims proporcionados.
        /// </summary>
        /// <param name="claims">Claims personalizados</param>
        /// <returns>Token generado</returns>
        public TokenResult CreateToken(List<Claim> claims)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));
            var tokenCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            DateTime expiration = DateTime.UtcNow.AddMinutes(jwtOptions.ExpirationInMinutes);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audience,
                SigningCredentials = tokenCredentials
            };
            var tokenHandler = new JsonWebTokenHandler();

            return new TokenResult(true, tokenHandler.CreateToken(tokenDescriptor), expiration);
        }

        /// <summary>
        /// Genera un refresh token seguro.
        /// </summary>
        /// <returns>Token generado</returns>
        public TokenResult GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            string refreshToken = Convert.ToBase64String(randomNumber);
            DateTime expiration = DateTime.UtcNow.AddDays(7);
            return new TokenResult(true, refreshToken, expiration);
        }
    }

    public static class TokenFactoryExtensions
    {
        /// <summary>
        /// Registra las dependencias del servicio de creación de tokens.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder RegisterTokenFactoryService(this WebApplicationBuilder builder)
        {
            JwtOptions jwtOptions = new(
                builder.Configuration.GetValue<string>("Jwt:Issuer") ?? throw new ArgumentNullException("Jwt:Issuer no está configurado apropiadamente en los appsettings."),
                builder.Configuration.GetValue<string>("Jwt:Audience") ?? throw new ArgumentNullException("Jwt:Audience no está configurado apropiadamente en los appsettings."),
                builder.Configuration.GetValue<int>("Jwt:ExpirationInMinutes"),
                builder.Configuration.GetValue<string>("Jwt:SecretKey") ?? throw new ArgumentNullException("Jwt:SecretKey no está configurado apropiadamente en los appsettings.")
            );
            builder.Services.AddSingleton(jwtOptions)
                            .AddScoped<TokenFactoryService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });

            builder.CreatePolicies();
            
            return builder;
        }

        public static WebApplicationBuilder CreatePolicies(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<RolePermissionsProvider>();
            RolePermissionsProvider rolePermissionsProvider = builder.Services.BuildServiceProvider().GetRequiredService<RolePermissionsProvider>();
            rolePermissionsProvider.LoadPermissions();

            builder.Services.AddAuthorizationBuilder()
                            .CreatePolicy(rolePermissionsProvider, "CanCreateEmployee")
                            .CreatePolicy(rolePermissionsProvider, "CanModifyEmployee")
                            .CreatePolicy(rolePermissionsProvider, "CanRemoveEmployee")
                            .CreatePolicy(rolePermissionsProvider, "CanRevokeEmployeeTokens")
                            .CreatePolicy(rolePermissionsProvider, "CanCreateCustomer")
                            .CreatePolicy(rolePermissionsProvider, "CanModifyCustomer")
                            .CreatePolicy(rolePermissionsProvider, "CanRecoveryPassword")
                            .CreatePolicy(rolePermissionsProvider, "CanRefreshToken")
                            .CreatePolicy(rolePermissionsProvider, "CanCreateProduct")
                            .CreatePolicy(rolePermissionsProvider, "CanEditProduct")
                            .CreatePolicy(rolePermissionsProvider, "CanCreateInvoice")
                            .CreatePolicy(rolePermissionsProvider, "CanCancelInvoice")
                            .CreatePolicy(rolePermissionsProvider, "CanListCustomers")
                            .CreatePolicy(rolePermissionsProvider, "CanListProducts")
                            .CreatePolicy(rolePermissionsProvider, "CanListEmployees")
                            .CreatePolicy(rolePermissionsProvider, "CanListInvoices");

            return builder;
        }

        /// <summary>
        /// Crea una política de autorización basada en un permiso específico utilizando el RolePermissionsProvider.
        /// </summary>
        /// <param name="builder">Constructor de autorización</param>
        /// <param name="provider">Proveedor de permisos por rol</param>
        /// <param name="permissionName">Nombre del permiso</param>
        /// <returns></returns>
        public static AuthorizationBuilder CreatePolicy(this AuthorizationBuilder builder, RolePermissionsProvider provider, string permissionName)
        {
            builder.AddPolicy(permissionName, policy =>
            {
                policy.RequireAssertion(context =>
                {
                    return context.IsAuthorized(provider, permissionName);
                });
            });

            return builder;
        }

        /// <summary>
        /// Verifica si el usuario tiene el permiso requerido utilizando el RolePermissionsProvider.
        /// </summary>
        /// <param name="context">Contexto de autorización</param>
        /// <param name="provider">Proveedor de permisos por rol</param>
        /// <param name="permission">Nombre del permiso</param>
        /// <returns>Resultado validación</returns>
        public static bool IsAuthorized(this AuthorizationHandlerContext context, RolePermissionsProvider provider,string permission)
        {
            var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;
            if (roleClaim == null) return false;

            return provider.HasPermission(roleClaim, permission);
        }

        /// <summary>
        /// Usa el servicio de creación de tokens en la aplicación.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static WebApplication UseTokenFactory(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}