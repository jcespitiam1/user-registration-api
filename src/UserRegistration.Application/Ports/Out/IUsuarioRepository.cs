using UserRegistration.Application.DTOs;

namespace UserRegistration.Application.Ports.Out;

/// <summary>
/// Puerto de salida (secundario) hacia la persistencia de usuarios.
/// Toda implementación debe apoyarse únicamente en stored procedures.
/// </summary>
public interface IUsuarioRepository
{
    Task<UsuarioResponse> RegistrarAsync(RegistrarUsuarioRequest request, CancellationToken cancellationToken);

    Task<UsuarioResponse?> ObtenerPorNumeroDocumentoAsync(string numeroDocumento, CancellationToken cancellationToken);
}
