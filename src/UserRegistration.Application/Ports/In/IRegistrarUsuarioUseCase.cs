using UserRegistration.Application.DTOs;

namespace UserRegistration.Application.Ports.In;

/// <summary>Puerto de entrada (primario): registrar un usuario.</summary>
public interface IRegistrarUsuarioUseCase
{
    Task<UsuarioResponse> EjecutarAsync(RegistrarUsuarioRequest request, CancellationToken cancellationToken);
}
