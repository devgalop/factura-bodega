using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using devgalop.facturabodega.webapi.Features.Users.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using Microsoft.EntityFrameworkCore;

namespace devgalop.facturabodega.webapi.Infrastructure.Persistence
{
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Agrega el contexto de la base de datos a los servicios de la aplicación.
        /// </summary>
        /// <param name="builder">builder de aplicación</param>
        public static WebApplicationBuilder AddDatabaseContext(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDatabaseContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("Postgres");
                options.UseNpgsql(connectionString)
                        .UseSnakeCaseNamingConvention();
            });
            return builder;
        }

        /// <summary>
        /// Verifica la conexión a la base de datos y registra el resultado en los logs.
        /// </summary>
        /// <param name="app">aplicación web</param>
        /// <returns></returns>
        public static async Task EnsureDatabaseCanConnectAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDatabaseContext>();
            var canConnect = await db.Database.CanConnectAsync();
            app.Logger.LogInformation("Can connect to database: {CanConnect}", canConnect);
        }

        /// <summary>
        /// Aplica las migraciones pendientes a la base de datos.
        /// </summary>
        /// <param name="app">aplicación web</param>
        /// <returns></returns>
        public static async Task ApplyMigrationsAsync(this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();
            await using AppDatabaseContext appContext =
                scope.ServiceProvider.GetRequiredService<AppDatabaseContext>();

            try
            {
                await appContext.Database.MigrateAsync();
                app.Logger.LogInformation("Se aplicaron las migraciones de la base de datos correctamente.");
            }
            catch (Exception e)
            {
                app.Logger.LogError(e, "Ocurrió un error al aplicar las migraciones de la base de datos.");
                throw;
            }
        }

        /// <summary>
        /// Reinicia la base de datos eliminándola y aplicando las migraciones.
        /// Solo para uso en desarrollo o pruebas.
        /// </summary>
        /// <param name="app">aplicación web</param>
        /// <returns></returns>
        public static async Task ResetDatabaseAsync(this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();
            await using AppDatabaseContext appContext =
                scope.ServiceProvider.GetRequiredService<AppDatabaseContext>();

            try
            {
                await appContext.Database.EnsureDeletedAsync();
                await appContext.Database.MigrateAsync();
                app.Logger.LogInformation("Se reinició la base de datos correctamente.");
            }
            catch (Exception e)
            {
                app.Logger.LogError(e, "Ocurrió un error al reiniciar la base de datos.");
                throw;
            }
        }

        /// <summary>
        /// Si la base de datos está vacía, inserta datos iniciales.
        /// </summary>
        /// <param name="app">aplicación web</param>
        /// <returns></returns>
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();
            await using AppDatabaseContext appContext =
                scope.ServiceProvider.GetRequiredService<AppDatabaseContext>();
            
            IPasswordManager<EmployeeCredentials> credentialsManager = 
                scope.ServiceProvider.GetRequiredService<IPasswordManager<EmployeeCredentials>>();
            using var transaction = await appContext.Database.BeginTransactionAsync();
            try
            {
                if (await appContext.Employees.AnyAsync() || await appContext.Roles.AnyAsync() || await appContext.Permissions.AnyAsync())
                {
                    app.Logger.LogInformation("La base de datos ya contiene datos. No se realizará el sembrado inicial.");
                    await transaction.RollbackAsync(); // Cerrar la transacción si no se realizan cambios
                    return;
                }

                // Crear permiso inicial
                PermissionEntity canCreateEmployeePermision = new("CanCreateEmployee");
                await appContext.Permissions.AddAsync(canCreateEmployeePermision);
                PermissionEntity canModifyEmployeePermision = new("CanModifyEmployee");
                await appContext.Permissions.AddAsync(canModifyEmployeePermision);
                PermissionEntity canRemoveEmployeePermision = new("CanRemoveEmployee");
                await appContext.Permissions.AddAsync(canRemoveEmployeePermision);
                PermissionEntity canRevokeEmployeePermision = new("CanRevokeEmployeeTokens");
                await appContext.Permissions.AddAsync(canRevokeEmployeePermision);
                PermissionEntity canCreateCustomerPermission = new("CanCreateCustomer");
                await appContext.Permissions.AddAsync(canCreateCustomerPermission);
                PermissionEntity canModifyCustomerPermission = new("CanModifyCustomer");
                await appContext.Permissions.AddAsync(canModifyCustomerPermission);
                PermissionEntity CanRecoveryPassword = new("CanRecoveryPassword");
                await appContext.Permissions.AddAsync(CanRecoveryPassword);
                PermissionEntity canRefeshTokenPermission = new("CanRefreshToken");
                await appContext.Permissions.AddAsync(canRefeshTokenPermission);
                

                // Crear rol inicial y asociar permiso
                RoleEntity adminRole = new("ADMIN");
                adminRole.Permissions.Add(canCreateEmployeePermision);
                adminRole.Permissions.Add(canModifyEmployeePermision);
                adminRole.Permissions.Add(canRemoveEmployeePermision);
                adminRole.Permissions.Add(canRevokeEmployeePermision);
                adminRole.Permissions.Add(canCreateCustomerPermission);
                adminRole.Permissions.Add(canModifyCustomerPermission);
                adminRole.Permissions.Add(CanRecoveryPassword);
                adminRole.Permissions.Add(canRefeshTokenPermission);
                await appContext.Roles.AddAsync(adminRole);

                RoleEntity simpleRole = new("BASIC");
                simpleRole.Permissions.Add(CanRecoveryPassword);
                simpleRole.Permissions.Add(canRefeshTokenPermission);
                await appContext.Roles.AddAsync(simpleRole);
                await appContext.SaveChangesAsync(); // Guardar para generar el ID del rol

                string samplePassword = "Password1234_*";
                string sampleEmail = "facu@yopmail.com";
                EmployeeCredentials credentials = new(sampleEmail, samplePassword);
                string hashedPassword = credentialsManager.HashPassword(credentials, samplePassword);
                // Crear empleado inicial y asociar rol
                EmployeeEntity initialEmployee = new(
                    name: "Facundo",
                    email: sampleEmail,
                    document: "123456789",
                    hiringDate: DateTime.UtcNow.AddYears(-1),
                    contractType: EmployeeContractType.FULL_TIME,
                    passwordHashed: hashedPassword,
                    role: adminRole
                );
                await appContext.Employees.AddAsync(initialEmployee);

                // Guardar cambios y confirmar transacción
                await appContext.SaveChangesAsync();
                await transaction.CommitAsync();

                app.Logger.LogInformation("Se insertaron los datos iniciales en la base de datos correctamente.");
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                app.Logger.LogError(e, "Ocurrió un error al sembrar los datos iniciales en la base de datos.");
                throw;
            }
        }
    }
}