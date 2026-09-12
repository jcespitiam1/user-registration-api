using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UserRegistration.Application.Ports.In;
using UserRegistration.Application.UseCases;
using UserRegistration.Application.Validators;

namespace UserRegistration.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegistrarUsuarioRequestValidator>();

        services.AddScoped<IRegistrarUsuarioUseCase, RegistrarUsuarioUseCase>();
        services.AddScoped<IObtenerUsuarioUseCase, ObtenerUsuarioUseCase>();
        services.AddScoped<IConsultaUbicacionesUseCase, ConsultaUbicacionesUseCase>();

        return services;
    }
}
