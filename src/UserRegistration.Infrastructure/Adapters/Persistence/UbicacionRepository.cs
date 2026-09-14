using Dapper;
using UserRegistration.Application.DTOs;
using UserRegistration.Application.Ports.Out;

namespace UserRegistration.Infrastructure.Adapters.Persistence;

public sealed class UbicacionRepository : IUbicacionRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UbicacionRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<PaisDto>> ListarPaisesAsync(CancellationToken cancellationToken)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = "SELECT * FROM registro.sp_pais_listar()";

        var comando = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var resultado = await connection.QueryAsync<PaisDto>(comando);
        return resultado.AsList();
    }

    public async Task<IReadOnlyList<DepartamentoDto>> ListarDepartamentosPorPaisAsync(int idPais, CancellationToken cancellationToken)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = "SELECT * FROM registro.sp_departamento_listar_por_pais(@IdPais)";

        var comando = new CommandDefinition(sql, new { IdPais = idPais }, cancellationToken: cancellationToken);
        var resultado = await connection.QueryAsync<DepartamentoDto>(comando);
        return resultado.AsList();
    }

    public async Task<IReadOnlyList<MunicipioDto>> ListarMunicipiosPorDepartamentoAsync(int idDepartamento, CancellationToken cancellationToken)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = "SELECT * FROM registro.sp_municipio_listar_por_departamento(@IdDepartamento)";

        var comando = new CommandDefinition(sql, new { IdDepartamento = idDepartamento }, cancellationToken: cancellationToken);
        var resultado = await connection.QueryAsync<MunicipioDto>(comando);
        return resultado.AsList();
    }

    public async Task<UbicacionValidacionResultado> ValidarUbicacionAsync(
        int idPais, int idDepartamento, int idMunicipio, CancellationToken cancellationToken)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = "SELECT * FROM registro.sp_ubicacion_validar(@IdPais, @IdDepartamento, @IdMunicipio)";

        var parametros = new { IdPais = idPais, IdDepartamento = idDepartamento, IdMunicipio = idMunicipio };
        var comando = new CommandDefinition(sql, parametros, cancellationToken: cancellationToken);

        return await connection.QuerySingleAsync<UbicacionValidacionResultado>(comando);
    }
}
