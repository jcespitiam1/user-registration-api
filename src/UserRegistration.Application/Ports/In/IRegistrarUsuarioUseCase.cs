using UserRegistration.Application.DTOs;

namespace UserRegistration.Application.Ports.In;

public interface IRegistrarUsuarioUseCase
{
    Task<UsuarioResponse> EjecutarAsync(RegistrarUsuarioRequest request, CancellationToken cancellationToken);
}
