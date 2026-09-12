using UserRegistration.Application.DTOs;

namespace UserRegistration.Application.Ports.In;

/// <summary>
/// Puerto de entrada (primario): consultar los catálogos paramétricos de
/// ubicación para alimentar, por ejemplo, listas desplegables en cascada.
/// </summary>
public interface IConsultaUbicacionesUseCase
{
    Task<IReadOnlyList<PaisDto>> ListarPaisesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<DepartamentoDto>> ListarDepartamentosAsync(int idPais, CancellationToken cancellationToken);

    Task<IReadOnlyList<MunicipioDto>> ListarMunicipiosAsync(int idDepartamento, CancellationToken cancellationToken);
}
