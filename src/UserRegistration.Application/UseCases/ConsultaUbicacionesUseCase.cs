using UserRegistration.Application.DTOs;
using UserRegistration.Application.Ports.In;
using UserRegistration.Application.Ports.Out;

namespace UserRegistration.Application.UseCases;

public sealed class ConsultaUbicacionesUseCase : IConsultaUbicacionesUseCase
{
    private readonly IUbicacionRepository _ubicacionRepository;

    public ConsultaUbicacionesUseCase(IUbicacionRepository ubicacionRepository)
    {
        _ubicacionRepository = ubicacionRepository;
    }

    public Task<IReadOnlyList<PaisDto>> ListarPaisesAsync(CancellationToken cancellationToken) =>
        _ubicacionRepository.ListarPaisesAsync(cancellationToken);

    public Task<IReadOnlyList<DepartamentoDto>> ListarDepartamentosAsync(int idPais, CancellationToken cancellationToken) =>
        _ubicacionRepository.ListarDepartamentosPorPaisAsync(idPais, cancellationToken);

    public Task<IReadOnlyList<MunicipioDto>> ListarMunicipiosAsync(int idDepartamento, CancellationToken cancellationToken) =>
        _ubicacionRepository.ListarMunicipiosPorDepartamentoAsync(idDepartamento, cancellationToken);
}
