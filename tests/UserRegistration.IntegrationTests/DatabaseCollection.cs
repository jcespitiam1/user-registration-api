using Xunit;

namespace UserRegistration.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class DatabaseCollection : ICollectionFixture<PostgresFixture>
{
    public const string Name = "Database";
}
