using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                options.UseNpgsql(connectionString);
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
    }
}