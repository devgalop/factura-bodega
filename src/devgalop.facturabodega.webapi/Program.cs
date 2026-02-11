
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Notifications.SendNotification;
using devgalop.facturabodega.webapi.Features.Products.Common;
using devgalop.facturabodega.webapi.Features.Users.Common;
using devgalop.facturabodega.webapi.Features.Users.Customers.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using FluentValidation;
using Scalar.AspNetCore;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

    Log.Information("Iniciando la aplicación...");   
    
    builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);
    builder.Services.AddOpenApi();
    
    builder.AddDatabaseContext()
            .AddMediator()
            .AddEndpoints()
            .RegisterEmployeeFeatures()
            .RegisterCustomerFeatures()
            .RegisterProductFeatures()
            .RegisterTokenFactoryService()
            .RegisterNotificationProvider()
            .AddExceptionHandlers();
    
    var app = builder.Build();
    
    app.UseSerilogRequestLogging();
    app.MapEndpoints();
    Log.Information("Validando conexión a la base de datos...");
    await app.EnsureDatabaseCanConnectAsync();
    
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference("/docs",options =>
        {
            options.Title = "API Sitema de facturación y gestión de bodega";
            options.WithOpenApiRoutePattern("/openapi/v1.json");
            options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            options.DarkMode = true;
        });
        app.MapGet("/", () => Results.Redirect("/docs"));
        //wait app.ApplyMigrationsAsync();
        Log.Information("Reiniciando y poblando la base de datos...");
        await app.ResetDatabaseAsync();
        await app.SeedDatabaseAsync();
    }
    
    app.UseExceptionHandler();
    app.UseHttpsRedirection();
    app.UseTokenFactory();
    
    app.Run();
    
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación ha finalizado inesperadamente.");
}
finally
{
    Log.CloseAndFlush();
}
