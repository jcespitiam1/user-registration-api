using UserRegistration.Application.DTOs;
using UserRegistration.Application.Ports.Out;
using UserRegistration.Domain.Exceptions;
using UserRegistration.Infrastructure.Adapters.Persistence;
using Xunit;

namespace UserRegistration.IntegrationTests;

[Collection(DatabaseCollection.Name)]
public sealed class UsuarioRepositoryTests
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUbicacionRepository _ubicacionRepository;

    public UsuarioRepositoryTests(PostgresFixture fixture)
    {
        _usuarioRepository = new UsuarioRepository(fixture.ConnectionFactory);
        _ubicacionRepository = new UbicacionRepository(fixture.ConnectionFactory);
    }

    private async Task<(int IdPais, int IdDepartamento, int IdMunicipio)> ResolverMedellinAsync()
    {
        var paises = await _ubicacionRepository.ListarPaisesAsync(CancellationToken.None);
        var colombia = paises.Single(p => p.Codigo == "CO");

        var departamentos = await _ubicacionRepository.ListarDepartamentosPorPaisAsync(colombia.IdPais, CancellationToken.None);
        var antioquia = departamentos.Single(d => d.Codigo == "ANT");

        var municipios = await _ubicacionRepository.ListarMunicipiosPorDepartamentoAsync(antioquia.IdDepartamento, CancellationToken.None);
        var medellin = municipios.Single(m => m.Codigo == "MED");

        return (colombia.IdPais, antioquia.IdDepartamento, medellin.IdMunicipio);
    }

    private static string NuevoNumeroDocumento() => Random.Shared.NextInt64(1_000_000_000, 9_999_999_999).ToString();

    [Fact]
    public async Task RegistrarAsync_persiste_y_ObtenerPorNumeroDocumentoAsync_devuelve_los_datos_con_ubicacion_resuelta()
    {
        var (idPais, idDepartamento, idMunicipio) = await ResolverMedellinAsync();
        var numeroDocumento = NuevoNumeroDocumento();

        var request = new RegistrarUsuarioRequest(
            numeroDocumento, "Juan Camilo Espitia", "+573001234567",
            idPais, idDepartamento, idMunicipio, "Calle 123 # 45-67");

        var registrado = await _usuarioRepository.RegistrarAsync(request, CancellationToken.None);

        Assert.Equal(numeroDocumento, registrado.NumeroDocumento);
        Assert.Equal("Colombia", registrado.PaisNombre);
        Assert.Equal("Antioquia", registrado.DepartamentoNombre);
        Assert.Equal("Medellín", registrado.MunicipioNombre);

        var consultado = await _usuarioRepository.ObtenerPorNumeroDocumentoAsync(numeroDocumento, CancellationToken.None);

        Assert.NotNull(consultado);
        Assert.Equal(registrado.NumeroDocumento, consultado!.NumeroDocumento);
        Assert.Equal(registrado.FechaRegistro, consultado.FechaRegistro);
    }

    [Fact]
    public async Task ObtenerPorNumeroDocumentoAsync_devuelve_null_si_no_existe()
    {
        var resultado = await _usuarioRepository.ObtenerPorNumeroDocumentoAsync("00000000000", CancellationToken.None);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task RegistrarAsync_con_numero_documento_duplicado_lanza_ConflictException()
    {
        var (idPais, idDepartamento, idMunicipio) = await ResolverMedellinAsync();
        var request = new RegistrarUsuarioRequest(
            NuevoNumeroDocumento(), "Ana Torres", "+573109876543",
            idPais, idDepartamento, idMunicipio, "Cra 50 # 10-20");

        await _usuarioRepository.RegistrarAsync(request, CancellationToken.None);

        await Assert.ThrowsAsync<ConflictException>(
            () => _usuarioRepository.RegistrarAsync(request, CancellationToken.None));
    }
}
