using Dapper;
using Npgsql;
using UserRegistration.Application.DTOs;
using UserRegistration.Application.Ports.Out;
using UserRegistration.Domain.Exceptions;

namespace UserRegistration.Infrastructure.Adapters.Persistence;

/// <summary>
/// Adaptador secundario: implementa el puerto de salida IUsuarioRepository
/// consumiendo exclusivamente stored procedures (funciones) de PostgreSQL.
/// Postgres no soporta CALL sobre funciones que retornan conjuntos de
/// filas, por lo que se invocan con SELECT * FROM schema.funcion(...),
/// que es la forma estándar de consumirlas desde un cliente ADO.NET.
/// </summary>
public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UsuarioRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<UsuarioResponse> RegistrarAsync(RegistrarUsuarioRequest request, CancellationToken cancellationToken)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = "SELECT * FROM registro.sp_usuario_registrar(" +
                            "@NumeroDocumento, @Nombre, @Telefono, @IdPais, @IdDepartamento, @IdMunicipio, @Direccion)";

        var comando = new CommandDefinition(sql, request, cancellationToken: cancellationToken);

        try
        {
            return await connection.QuerySingleAsync<UsuarioResponse>(comando);
        }
        catch (PostgresException ex) when (ex.SqlState == "23505")
        {
            throw new ConflictException(
                $"Ya existe un usuario registrado con el número de documento '{request.NumeroDocumento}'.");
        }
        catch (PostgresException ex) when (ex.SqlState == "23503")
        {
            throw new ConflictException(
                "El dato entra en conflicto con las restricciones de integridad de la base de datos.");
        }
    }

    public async Task<UsuarioResponse?> ObtenerPorNumeroDocumentoAsync(string numeroDocumento, CancellationToken cancellationToken)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = "SELECT * FROM registro.sp_usuario_obtener(@NumeroDocumento)";

        var comando = new CommandDefinition(
            sql, new { NumeroDocumento = numeroDocumento }, cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<UsuarioResponse>(comando);
    }
}
