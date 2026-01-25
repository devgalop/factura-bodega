using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devgalop.facturabodega.webapi.Features.Users.Common
{

    /// <summary>
    /// Administrador de contraseñas hasheadas
    /// </summary>
    /// <typeparam name="TUser">Tipo de usuario</typeparam>
    public interface IPasswordManager<TUser>
    {
        /// <summary>
        /// Hashea la contraseña proporcionada
        /// </summary>
        /// <param name="user">Entidad de usuario</param>
        /// <param name="password">Contraseña sin encriptar</param>
        /// <returns>Contraseña encriptada</returns>
        string HashPassword(TUser user, string password);
        /// <summary>
        /// Verifica si la contraseña proporcionada coincide con la contraseña hasheada
        /// </summary>
        /// <param name="user">Entidad de usuario</param>
        /// <param name="hashedPassword">Contraseña encriptada</param>
        /// <param name="providedPassword">Contraseña sin encriptar</param>
        /// <returns>Resultado de validación</returns>
        bool VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword);
    }
}