using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace devgalop.facturabodega.webapi.Features.Users.Common
{
    public record JwtOptions(string Issuer, string Audience, int ExpirationInMinutes, string SecretKey);
    public record TokenResult(bool IsSuccessful, string Token, DateTime Expiration);

    public class TokenFactoryService(IConfiguration configuration)
    {
        private readonly JwtOptions _jwtOptions = new(
            configuration.GetValue<string>("Jwt:Issuer") ?? throw new ArgumentNullException("Jwt:Issuer no está configurado apropiadamente en los appsettings."),
            configuration.GetValue<string>("Jwt:Audience") ?? throw new ArgumentNullException("Jwt:Audience no está configurado apropiadamente en los appsettings."),
            configuration.GetValue<int>("Jwt:ExpirationInMinutes"),
            configuration.GetValue<string>("Jwt:SecretKey") ?? throw new ArgumentNullException("Jwt:SecretKey no está configurado apropiadamente en los appsettings.")
        );
        
        /// <summary>
        /// Crea un token JWT con los claims proporcionados.
        /// </summary>
        /// <param name="claims">Claims personalizados</param>
        /// <returns>Token generado</returns>
        public TokenResult CreateToken(List<Claim> claims)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var tokenCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
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

            return new TokenResult(true, tokenHandler.CreateToken(tokenDescriptor), expiration);
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
            builder.Services.AddScoped<TokenFactoryService>();
            return builder;
        }
    }
}