using UserRegistration.Application.DTOs;

namespace UserRegistration.Application.Ports.Out;

/// <summary>
/// Puerto de salida (secundario) hacia las tablas paramétricas de
/// ubicación (país, departamento, municipio).
/// </summary>
public interface IUbicacionRepository
{
    Task<IReadOnlyList<PaisDto>> ListarPaisesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<DepartamentoDto>> ListarDepartamentosPorPaisAsync(int idPais, CancellationToken cancellationToken);

    Task<IReadOnlyList<MunicipioDto>> ListarMunicipiosPorDepartamentoAsync(int idDepartamento, CancellationToken cancellationToken);

    Task<UbicacionValidacionResultado> ValidarUbicacionAsync(
        int idPais, int idDepartamento, int idMunicipio, CancellationToken cancellationToken);
}
