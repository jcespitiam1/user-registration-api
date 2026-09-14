using Microsoft.AspNetCore.Mvc.Testing;

namespace UserRegistration.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public CustomWebApplicationFactory(string connectionString)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__Postgres", connectionString);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Environment.SetEnvironmentVariable("ConnectionStrings__Postgres", null);
    }
}
