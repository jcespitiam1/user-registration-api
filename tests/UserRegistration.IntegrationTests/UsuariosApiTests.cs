using System.Net;
using System.Net.Http.Json;
using UserRegistration.Application.DTOs;
using Xunit;

namespace UserRegistration.IntegrationTests;

[Collection(DatabaseCollection.Name)]
public sealed class UsuariosApiTests : IAsyncLifetime
{
    private readonly PostgresFixture _fixture;
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    public UsuariosApiTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync()
    {
        _factory = new CustomWebApplicationFactory(_fixture.ConnectionString);
        _client = _factory.CreateClient();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        _factory.Dispose();
        return Task.CompletedTask;
    }

    private async Task<(int IdPais, int IdDepartamento, int IdMunicipio)> ResolverMedellinAsync()
    {
        var paises = await _client.GetFromJsonAsync<List<PaisDto>>("/api/paises");
        var colombia = paises!.Single(p => p.Codigo == "CO");

        var departamentos = await _client.GetFromJsonAsync<List<DepartamentoDto>>($"/api/paises/{colombia.IdPais}/departamentos");
        var antioquia = departamentos!.Single(d => d.Codigo == "ANT");

        var municipios = await _client.GetFromJsonAsync<List<MunicipioDto>>($"/api/departamentos/{antioquia.IdDepartamento}/municipios");
        var medellin = municipios!.Single(m => m.Codigo == "MED");

        return (colombia.IdPais, antioquia.IdDepartamento, medellin.IdMunicipio);
    }

    private static string NuevoNumeroDocumento() => Random.Shared.NextInt64(1_000_000_000, 9_999_999_999).ToString();

    [Fact]
    public async Task POST_usuarios_con_datos_validos_devuelve_201_y_el_usuario_creado()
    {
        var (idPais, idDepartamento, idMunicipio) = await ResolverMedellinAsync();
        var numeroDocumento = NuevoNumeroDocumento();

        var response = await _client.PostAsJsonAsync("/api/usuarios", new
        {
            numeroDocumento,
            nombre = "Juan Camilo Espitia",
            telefono = "+573001234567",
            idPais,
            idDepartamento,
            idMunicipio,
            direccion = "Calle 123 # 45-67"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var usuario = await response.Content.ReadFromJsonAsync<UsuarioResponse>();
        Assert.Equal(numeroDocumento, usuario!.NumeroDocumento);
        Assert.Equal("Medellín", usuario.MunicipioNombre);
    }

    [Fact]
    public async Task POST_usuarios_con_telefono_invalido_devuelve_400_con_el_campo_marcado()
    {
        var (idPais, idDepartamento, idMunicipio) = await ResolverMedellinAsync();

        var response = await _client.PostAsJsonAsync("/api/usuarios", new
        {
            numeroDocumento = NuevoNumeroDocumento(),
            nombre = "Juan Perez",
            telefono = "abc",
            idPais,
            idDepartamento,
            idMunicipio,
            direccion = "Calle 1"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Telefono", body);
    }

    [Fact]
    public async Task POST_usuarios_con_municipio_de_otro_departamento_devuelve_400()
    {
        var (idPais, _, idMunicipioMedellin) = await ResolverMedellinAsync();
        var departamentos = await _client.GetFromJsonAsync<List<DepartamentoDto>>($"/api/paises/{idPais}/departamentos");
        var cundinamarca = departamentos!.Single(d => d.Codigo == "CUN");

        var response = await _client.PostAsJsonAsync("/api/usuarios", new
        {
            numeroDocumento = NuevoNumeroDocumento(),
            nombre = "Juan Perez",
            telefono = "+573001234567",
            idPais,
            idDepartamento = cundinamarca.IdDepartamento,
            idMunicipio = idMunicipioMedellin,
            direccion = "Calle 1"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task POST_usuarios_con_numero_documento_duplicado_devuelve_409()
    {
        var (idPais, idDepartamento, idMunicipio) = await ResolverMedellinAsync();
        var payload = new
        {
            numeroDocumento = NuevoNumeroDocumento(),
            nombre = "Ana Torres",
            telefono = "+573109876543",
            idPais,
            idDepartamento,
            idMunicipio,
            direccion = "Cra 50 # 10-20"
        };

        await _client.PostAsJsonAsync("/api/usuarios", payload);
        var response = await _client.PostAsJsonAsync("/api/usuarios", payload);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GET_usuarios_con_documento_inexistente_devuelve_404()
    {
        var response = await _client.GetAsync("/api/usuarios/00000000000");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
