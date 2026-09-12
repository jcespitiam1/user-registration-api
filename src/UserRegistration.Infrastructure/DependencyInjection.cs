using Dapper;
using Microsoft.Extensions.DependencyInjection;
using UserRegistration.Application.Ports.Out;
using UserRegistration.Infrastructure.Adapters.Persistence;

namespace UserRegistration.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        // Permite que Dapper resuelva columnas snake_case (id_pais) hacia
        // propiedades PascalCase (IdPais) sin necesidad de alias manuales.
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IUbicacionRepository, UbicacionRepository>();

        return services;
    }
}
