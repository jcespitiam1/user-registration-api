using Npgsql;
using Testcontainers.PostgreSql;
using UserRegistration.Infrastructure.Adapters.Persistence;
using Xunit;

namespace UserRegistration.IntegrationTests;

public sealed class PostgresFixture : IAsyncLifetime
{
    private static readonly string[] Scripts =
    [
        "01_create_tables.sql",
        "02_seed_data.sql",
        "03_stored_procedures.sql"
    ];

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("registro_usuarios")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public IDbConnectionFactory ConnectionFactory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        ConnectionFactory = new NpgsqlConnectionFactory(ConnectionString);

        foreach (var script in Scripts)
        {
            var sql = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Database", script));

            await using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            await command.ExecuteNonQueryAsync();
        }
    }

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();
}
