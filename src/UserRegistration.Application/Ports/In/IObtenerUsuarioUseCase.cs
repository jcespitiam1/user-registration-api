using UserRegistration.Application.DTOs;

namespace UserRegistration.Application.Ports.In;

/// <summary>Puerto de entrada (primario): consultar un usuario ya registrado.</summary>
public interface IObtenerUsuarioUseCase
{
    Task<UsuarioResponse> EjecutarAsync(string numeroDocumento, CancellationToken cancellationToken);
}
