using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using FluentValidation;

namespace devgalop.facturabodega.webapi.Features.Users.Employees.AddEmployee;

public class AddEmployeeHandler(AppDatabaseContext dbContext) : ICommandHandler<AddEmployeeCommand>
{
    private readonly AppDatabaseContext _dbContext = dbContext;

    public async Task HandleAsync(AddEmployeeCommand command)
    {
        var newEmployee = new EmployeeEntity(
            command.Name,
            command.Email,
            command.HiringDate,
            command.ContractType);

        await _dbContext.Employees.AddAsync(newEmployee);
        await _dbContext.SaveChangesAsync();
    }
}

public record AddEmployeeCommand(string Name, string Email, DateTime HiringDate, EmployeeContractType ContractType) : ICommand;

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
