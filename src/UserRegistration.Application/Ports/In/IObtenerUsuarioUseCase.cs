using UserRegistration.Application.DTOs;

namespace UserRegistration.Application.Ports.In;

public interface IObtenerUsuarioUseCase
{
    Task<UsuarioResponse> EjecutarAsync(string numeroDocumento, CancellationToken cancellationToken);
}
