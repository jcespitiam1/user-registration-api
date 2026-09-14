using UserRegistration.Application.DTOs;

namespace UserRegistration.Application.Ports.In;

public interface IConsultaUbicacionesUseCase
{
    Task<IReadOnlyList<PaisDto>> ListarPaisesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<DepartamentoDto>> ListarDepartamentosAsync(int idPais, CancellationToken cancellationToken);

    Task<IReadOnlyList<MunicipioDto>> ListarMunicipiosAsync(int idDepartamento, CancellationToken cancellationToken);
}
