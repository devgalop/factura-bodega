using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            try
            {
                if(await appContext.Employees.AnyAsync())
                {
                    app.Logger.LogInformation("La base de datos ya contiene datos. No se realizará el sembrado inicial.");
                    return;
                }
                EmployeeEntity initialEmployee = new(
                    name: "Juan Pérez",
                    email: "jperez@yopmail.com",
                    hiringDate: DateTime.UtcNow.AddYears(-1),
                    contractType: EmployeeContractType.FULL_TIME
                );
                await appContext.Employees.AddAsync(initialEmployee);
                await appContext.SaveChangesAsync();

                app.Logger.LogInformation("Se insertaron los datos iniciales en la base de datos correctamente.");
            }
            catch (Exception e)
            {
                app.Logger.LogError(e, "Ocurrió un error al sembrar los datos iniciales en la base de datos.");
                throw;
            }
        }
    }
}