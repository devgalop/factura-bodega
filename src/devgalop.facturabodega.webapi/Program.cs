
using devgalop.facturabodega.webapi.Common;
using devgalop.facturabodega.webapi.Features.Users.Employees.Common;
using devgalop.facturabodega.webapi.Infrastructure.Persistence;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);
builder.Services.AddOpenApi();

builder.AddDatabaseContext()
        .AddMediator()
        .AddEndpoints()
        .RegisterEmployeeFeatures()
        .AddExceptionHandlers();

var app = builder.Build();

app.MapEndpoints();
await app.EnsureDatabaseCanConnectAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await app.ApplyMigrationsAsync();
    await app.SeedDatabaseAsync();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.Run();

