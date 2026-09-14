using UserRegistration.Application.Ports.Out;
using UserRegistration.Infrastructure.Adapters.Persistence;
using Xunit;

namespace UserRegistration.IntegrationTests;

[Collection(DatabaseCollection.Name)]
public sealed class UbicacionRepositoryTests
{
    private readonly IUbicacionRepository _ubicacionRepository;

    public UbicacionRepositoryTests(PostgresFixture fixture)
    {
        _ubicacionRepository = new UbicacionRepository(fixture.ConnectionFactory);
    }

    private async Task<(int IdPais, int IdDepartamento, int IdMunicipio)> ResolverAsync(
        string paisCodigo, string deptoCodigo, string municipioCodigo)
    {
        var paises = await _ubicacionRepository.ListarPaisesAsync(CancellationToken.None);
        var pais = paises.Single(p => p.Codigo == paisCodigo);

        var departamentos = await _ubicacionRepository.ListarDepartamentosPorPaisAsync(pais.IdPais, CancellationToken.None);
        var departamento = departamentos.Single(d => d.Codigo == deptoCodigo);

        var municipios = await _ubicacionRepository.ListarMunicipiosPorDepartamentoAsync(departamento.IdDepartamento, CancellationToken.None);
        var municipio = municipios.Single(m => m.Codigo == municipioCodigo);

        return (pais.IdPais, departamento.IdDepartamento, municipio.IdMunicipio);
    }

    [Fact]
    public async Task ListarPaisesAsync_incluye_los_paises_sembrados()
    {
        var paises = await _ubicacionRepository.ListarPaisesAsync(CancellationToken.None);

        Assert.Contains(paises, p => p.Codigo == "CO" && p.Nombre == "Colombia");
        Assert.Contains(paises, p => p.Codigo == "MX" && p.Nombre == "México");
        Assert.Contains(paises, p => p.Codigo == "PE" && p.Nombre == "Perú");
    }

    [Fact]
    public async Task ListarDepartamentosPorPaisAsync_solo_devuelve_departamentos_del_pais_indicado()
    {
        var paises = await _ubicacionRepository.ListarPaisesAsync(CancellationToken.None);
        var colombia = paises.Single(p => p.Codigo == "CO");
        var mexico = paises.Single(p => p.Codigo == "MX");

        var departamentosColombia = await _ubicacionRepository.ListarDepartamentosPorPaisAsync(colombia.IdPais, CancellationToken.None);

        Assert.Contains(departamentosColombia, d => d.Codigo == "ANT");
        Assert.DoesNotContain(departamentosColombia, d => d.IdPais == mexico.IdPais);
    }

    [Fact]
    public async Task ValidarUbicacionAsync_con_jerarquia_coherente_devuelve_todo_verdadero()
    {
        var (idPais, idDepartamento, idMunicipio) = await ResolverAsync("CO", "ANT", "MED");

        var resultado = await _ubicacionRepository.ValidarUbicacionAsync(idPais, idDepartamento, idMunicipio, CancellationToken.None);

        Assert.True(resultado.PaisExiste);
        Assert.True(resultado.DepartamentoExiste);
        Assert.True(resultado.DepartamentoPerteneceAPais);
        Assert.True(resultado.MunicipioExiste);
        Assert.True(resultado.MunicipioPerteneceADepartamento);
    }

    [Fact]
    public async Task ValidarUbicacionAsync_con_municipio_de_otro_departamento_marca_incoherencia()
    {
        var (idPais, idDepartamentoCundinamarca, _) = await ResolverAsync("CO", "CUN", "ZIP");
        var (_, _, idMunicipioMedellin) = await ResolverAsync("CO", "ANT", "MED");

        var resultado = await _ubicacionRepository.ValidarUbicacionAsync(
            idPais, idDepartamentoCundinamarca, idMunicipioMedellin, CancellationToken.None);

        Assert.True(resultado.PaisExiste);
        Assert.True(resultado.DepartamentoExiste);
        Assert.True(resultado.MunicipioExiste);
        Assert.False(resultado.MunicipioPerteneceADepartamento);
    }

    [Fact]
    public async Task ValidarUbicacionAsync_con_ids_inexistentes_marca_no_existe()
    {
        var resultado = await _ubicacionRepository.ValidarUbicacionAsync(999999, 999999, 999999, CancellationToken.None);

        Assert.False(resultado.PaisExiste);
        Assert.False(resultado.DepartamentoExiste);
        Assert.False(resultado.MunicipioExiste);
    }
}
