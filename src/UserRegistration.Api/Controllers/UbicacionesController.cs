using Microsoft.AspNetCore.Mvc;
using UserRegistration.Application.DTOs;
using UserRegistration.Application.Ports.In;

namespace UserRegistration.Api.Controllers;

[ApiController]
[Route("api")]
[Produces("application/json")]
public sealed class UbicacionesController : ControllerBase
{
    private readonly IConsultaUbicacionesUseCase _consultaUbicacionesUseCase;

    public UbicacionesController(IConsultaUbicacionesUseCase consultaUbicacionesUseCase)
    {
        _consultaUbicacionesUseCase = consultaUbicacionesUseCase;
    }

    [HttpGet("paises")]
    [ProducesResponseType(typeof(IReadOnlyList<PaisDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PaisDto>>> ListarPaises(CancellationToken cancellationToken)
    {
        return Ok(await _consultaUbicacionesUseCase.ListarPaisesAsync(cancellationToken));
    }

    [HttpGet("paises/{idPais:int}/departamentos")]
    [ProducesResponseType(typeof(IReadOnlyList<DepartamentoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DepartamentoDto>>> ListarDepartamentos(
        int idPais, CancellationToken cancellationToken)
    {
        return Ok(await _consultaUbicacionesUseCase.ListarDepartamentosAsync(idPais, cancellationToken));
    }

    [HttpGet("departamentos/{idDepartamento:int}/municipios")]
    [ProducesResponseType(typeof(IReadOnlyList<MunicipioDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MunicipioDto>>> ListarMunicipios(
        int idDepartamento, CancellationToken cancellationToken)
    {
        return Ok(await _consultaUbicacionesUseCase.ListarMunicipiosAsync(idDepartamento, cancellationToken));
    }
}
