using UserRegistration.Application.DTOs;
using UserRegistration.Application.Ports.In;
using UserRegistration.Application.Ports.Out;
using UserRegistration.Domain.Exceptions;

namespace UserRegistration.Application.UseCases;

public sealed class ObtenerUsuarioUseCase : IObtenerUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;

    public ObtenerUsuarioUseCase(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<UsuarioResponse> EjecutarAsync(int idUsuario, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(idUsuario, cancellationToken);
        return usuario ?? throw NotFoundException.For("Usuario", idUsuario);
    }
}
