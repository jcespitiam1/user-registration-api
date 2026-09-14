using UserRegistration.Application.DTOs;

namespace UserRegistration.Application.Ports.Out;

public interface IUbicacionRepository
{
    Task<IReadOnlyList<PaisDto>> ListarPaisesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<DepartamentoDto>> ListarDepartamentosPorPaisAsync(int idPais, CancellationToken cancellationToken);

    Task<IReadOnlyList<MunicipioDto>> ListarMunicipiosPorDepartamentoAsync(int idDepartamento, CancellationToken cancellationToken);

    Task<UbicacionValidacionResultado> ValidarUbicacionAsync(
        int idPais, int idDepartamento, int idMunicipio, CancellationToken cancellationToken);
}
