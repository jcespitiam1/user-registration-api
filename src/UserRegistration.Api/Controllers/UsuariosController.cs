using Microsoft.AspNetCore.Mvc;
using UserRegistration.Application.DTOs;
using UserRegistration.Application.Ports.In;

namespace UserRegistration.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Produces("application/json")]
public sealed class UsuariosController : ControllerBase
{
    private readonly IRegistrarUsuarioUseCase _registrarUsuarioUseCase;
    private readonly IObtenerUsuarioUseCase _obtenerUsuarioUseCase;

    public UsuariosController(
        IRegistrarUsuarioUseCase registrarUsuarioUseCase,
        IObtenerUsuarioUseCase obtenerUsuarioUseCase)
    {
        _registrarUsuarioUseCase = registrarUsuarioUseCase;
        _obtenerUsuarioUseCase = obtenerUsuarioUseCase;
    }

    /// <summary>Registra un nuevo usuario con su ubicación (país, departamento, municipio).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> Registrar(
        [FromBody] RegistrarUsuarioRequest request, CancellationToken cancellationToken)
    {
        var usuario = await _registrarUsuarioUseCase.EjecutarAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorId), new { idUsuario = usuario.IdUsuario }, usuario);
    }

    /// <summary>Consulta un usuario previamente registrado.</summary>
    [HttpGet("{idUsuario:int}")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioResponse>> ObtenerPorId(int idUsuario, CancellationToken cancellationToken)
    {
        var usuario = await _obtenerUsuarioUseCase.EjecutarAsync(idUsuario, cancellationToken);
        return Ok(usuario);
    }
}
