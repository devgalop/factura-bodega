using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Notifications.Common;
using devgalop.facturabodega.webapi.Features.Notifications.SendNotification;
using devgalop.facturabodega.webapi.Features.Users.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using FluentValidation;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.AddEmployee;

public record AddEmployeeNotificationParams(string CustomerName) : INotificationParams;

public sealed class AddEmployeeHandler(
    IGetEmployeeRepository getEmployeeRepository,
    ICreateEmployeeRepository createEmployeeRepository, 
    IGetRoleRepository getRoleRepository,
    IPasswordManager<EmployeeCredentials> passwordManager,
    NotificationProvider notificationProvider) : ICommandHandler<AddEmployeeCommand>
{
    public async Task HandleAsync(AddEmployeeCommand command)
    {
        bool isAlreadyRegistered = await getEmployeeRepository.GetEmployeeByEmail(command.Email) != null;
        if(isAlreadyRegistered)
        {
            throw new ValidationException(
                [
                    new FluentValidation.Results.ValidationFailure(
                        nameof(command.Email),
                        "El correo electrónico proporcionado ya está registrado.")
                ]);
        }
        
        var role = await getRoleRepository.GetRoleByName("BASIC") ?? throw new Exception("El rol no se encontró en la base de datos.");
            
        EmployeeCredentials credentials = new(command.Email, command.Password);
        string hashedPassword = passwordManager.HashPassword(credentials, command.Password);
        
        var newEmployee = new EmployeeEntity(
            command.Name,
            command.Email,
            command.Document,
            hashedPassword,
            command.HiringDate,
            command.ContractType,
            role);

        await createEmployeeRepository.CreateEmployee(newEmployee);

        await notificationProvider.SendAsync(new NotificationContent(
            Subject: "Bienvenido al sistema",
            HtmlContent: $"<p>Hola {command.Name},</p><p>Has sido registrado exitosamente como empleado.</p>",
            Sender: new NotificationAddress("Sistema de Facturación", "devgalop@gmail.com"),
            To: new List<NotificationAddress> { new NotificationAddress(command.Name, command.Email) }
        ));
    }
}

public record AddEmployeeCommand(string Name, string Email, string Document, string Password, DateTime HiringDate, EmployeeContractType ContractType) : ICommand;

public static class AddEmployeeExtensions
{
    /// <summary>
    /// Registra la dependecia del handler para el feature de agregar empleado.
    /// </summary>
    /// <param name="builder">builder de aplicación</param>
    /// <returns></returns>
    public static WebApplicationBuilder RegisterAddEmployeeFeature(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICommandHandler<AddEmployeeCommand>, AddEmployeeHandler>();
        return builder;
    }
}
