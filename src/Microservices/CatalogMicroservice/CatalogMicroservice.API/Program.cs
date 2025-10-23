using Microservice.Catalog.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING", EnvironmentVariableTarget.Process) ?? "";

builder.Services.AddCatalogBrandRepository(connectionString);
builder.Services.AddCatalogItemRepository(connectionString);
builder.Services.AddCatalogTypeRepository(connectionString);

builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using var scope = app.Services.CreateScope();
scope.SetupCatalogDatabase();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
