using UserRegistration.Application.DTOs;

namespace UserRegistration.Application.Ports.Out;

public interface IUsuarioRepository
{
    Task<UsuarioResponse> RegistrarAsync(RegistrarUsuarioRequest request, CancellationToken cancellationToken);

    Task<UsuarioResponse?> ObtenerPorNumeroDocumentoAsync(string numeroDocumento, CancellationToken cancellationToken);
}
