using Microsoft.OpenApi.Models;
using UserRegistration.Api.Middleware;
using UserRegistration.Application;
using UserRegistration.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'Postgres'.");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Registro de Usuarios",
        Version = "v1",
        Description = "Servicio para registrar usuarios con país, departamento, municipio y dirección, " +
                       "validando formato y coherencia referencial de la ubicación."
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Registro de Usuarios v1");
});

app.MapControllers();

app.Run();
