using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Notifications.SendNotification;
using devgalop.facturabodega.webapi.Features.Users.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.RecoveryPassword
{
    public record RecoveryPasswordCommand(string Email): ICommand;

    public sealed class RecoveryPasswordHandler(
        AppDatabaseContext dbContext,
        TokenFactoryService tokenFactoryService,
        NotificationProvider notificationProvider
    ): ICommandHandler<RecoveryPasswordCommand>
    {
        public async Task HandleAsync(RecoveryPasswordCommand command)
        {
            var employeeFound = await dbContext.Employees
                                    .Where(e => e.Email == command.Email)
                                    .FirstOrDefaultAsync() 
                                    ?? throw new ValidationException(
                                        [
                                            new ValidationFailure(
                                                nameof(command.Email),
                                                "No se encontró ningún empleado con el correo electrónico proporcionado.")
                                        ]);
            
            var tokenGenerated = tokenFactoryService.GenerateOTPToken(60);
            var tokenFound = await dbContext.RecoverPasswordTokens
                                    .Where(t => t.EmployeeId == employeeFound.Id)
                                    .FirstOrDefaultAsync();

            await notificationProvider.SendAsync(new NotificationContent(
                Subject: "Recuperación de contraseña",
                HtmlContent: $"<p>Hola {employeeFound.Name},</p><p>Has solicitado recuperar tu contraseña. Por eso toma este token para que continues el proceso.</p><p><strong>{tokenGenerated.Token}</strong></p><p>Si no solicitaste este proceso, por favor ignora este correo.</p>",
                Sender: new NotificationAddress("Sistema de Facturación", "devgalop@gmail.com"),
                To: new List<NotificationAddress> { new NotificationAddress(employeeFound.Name, command.Email) }
            ));
            
            if (tokenFound is not null)
            {
                tokenFound.Token = tokenGenerated.Token;
                tokenFound.ExpiresOnUtc = tokenGenerated.Expiration;
                tokenFound.IsUsed = false;
                dbContext.RecoverPasswordTokens.Update(tokenFound);
                await dbContext.SaveChangesAsync();
                return;
            }

            RecoverPasswordTokenEntity tokenEntity = new(tokenGenerated.Token, tokenGenerated.Expiration, employeeFound);
            await dbContext.RecoverPasswordTokens.AddAsync(tokenEntity);
            await dbContext.SaveChangesAsync();
        }
    }

    public static class RecoveryPasswordExtensions
    {
        public static WebApplicationBuilder RegisterRecoveryPasswordFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICommandHandler<RecoveryPasswordCommand>, RecoveryPasswordHandler>();
            return builder;
        }
    }
}