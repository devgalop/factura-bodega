using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Users.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.ChangePassword
{
    /// <summary>
    /// Comando para cambiar la contraseña de un empleado existente.
    /// </summary>
    /// <param name="UserId">Identificador del usuario</param>
    /// <param name="CurrentPassword">Contraseña actual</param>
    /// <param name="NewPassword">Contraseña Nueva</param>
    public record ChangePasswordCommand(string UserId, string CurrentPassword, string NewPassword):ICommand;

    /// <summary>
    /// Handler para cambiar la contraseña de un empleado existente.
    /// </summary>
    /// <param name="dbContext">Contexto de base de datos</param>
    /// <param name="passwordManager">Administrador de contraseñas</param>
    public sealed class ChangePasswordHandler(
        AppDatabaseContext dbContext, 
        IPasswordManager<EmployeeCredentials> passwordManager
    ) : ICommandHandler<ChangePasswordCommand>
    {
        public async Task HandleAsync(ChangePasswordCommand command)
        {
            var employeeFound = await dbContext.Employees
                                                .FirstOrDefaultAsync(e => e.Id.ToString() == command.UserId)
                                                ?? throw new EmployeeNotFoundException(command.UserId);
            
            EmployeeCredentials currentCredentials = new(employeeFound.Email, command.CurrentPassword);
            if(!passwordManager.VerifyHashedPassword(currentCredentials,employeeFound.PasswordHashed,command.CurrentPassword))
                throw new ValidationException(
                                        [
                                            new ValidationFailure(
                                                nameof(command.CurrentPassword),
                                                "La contraseña proporcionada es incorrecta.")
                                        ]);
            
            EmployeeCredentials newCredentials = new(employeeFound.Email, command.NewPassword);
            string newHashedPassword = passwordManager.HashPassword(newCredentials, command.NewPassword);
            employeeFound.PasswordHashed = newHashedPassword;
            dbContext.Employees.Update(employeeFound);
            await dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Comando para cambiar la contraseña de un empleado existente utilizando un token de recuperación de contraseña.
    /// </summary>
    /// <param name="Token">Token de recuperación</param>
    /// <param name="NewPassword">Nueva contraseña</param>
    public record ChangePasswordWithTokenCommand(string Token, string NewPassword) : ICommand;

    /// <summary>
    /// Handler para cambiar la contraseña de un empleado existente utilizando un token de recuperación de contraseña.
    /// </summary>
    /// <param name="getEmployeeRepository">Repositorio para obtener empleados</param>
    /// <param name="editEmployeeRepository">Repositorio para editar empleados</param>
    /// <param name="passwordManager">Gestor de contraseñas</param>
    public sealed class ChangePasswordWithTokenHandler(
        IGetEmployeeRepository getEmployeeRepository,
        IEditEmployeeRepository editEmployeeRepository,
        IPasswordManager<EmployeeCredentials> passwordManager
    ): ICommandHandler<ChangePasswordWithTokenCommand>
    {
        public async Task HandleAsync(ChangePasswordWithTokenCommand command)
        {
            var employeeFound = await getEmployeeRepository.GetRecoverPasswordToken(command.Token)
                                                ?? throw new ValidationException(
                                                        [
                                                            new ValidationFailure(
                                                                nameof(command.Token),
                                                                "El token de recuperación es inválido.")
                                                        ]);
            if(employeeFound.ExpiresOnUtc < DateTime.UtcNow) throw new ValidationException(
                                        [
                                            new ValidationFailure(
                                                nameof(command.Token),
                                                "El token de recuperación ha expirado.")
                                        ]);

            if(employeeFound.IsUsed) throw new ValidationException(
                                        [
                                            new ValidationFailure(
                                                nameof(command.Token),
                                                "El token de recuperación ya ha sido utilizado.")
                                        ]);
            
            EmployeeCredentials newCredentials = new(employeeFound.Employee.Email, command.NewPassword);
            string newHashedPassword = passwordManager.HashPassword(newCredentials, command.NewPassword);
            employeeFound.Employee.PasswordHashed = newHashedPassword;
            await editEmployeeRepository.EditRecoveryToken(employeeFound);
            await editEmployeeRepository.EditEmployee(employeeFound.Employee);
        }
    }

    /// <summary>
    /// Extensiones para registrar el handler para cambiar la contraseña de un empleado existente en los servicios de la aplicación.
    /// </summary>
    public static class ChangePasswordExtensions
    {
        /// <summary>
        /// Registra el handler para cambiar la contraseña de un empleado existente en los servicios de la aplicación.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder RegisterChangePasswordFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICommandHandler<ChangePasswordCommand>, ChangePasswordHandler>()
                            .AddScoped<ICommandHandler<ChangePasswordWithTokenCommand>, ChangePasswordWithTokenHandler>();
            return builder;
        }
    }
}