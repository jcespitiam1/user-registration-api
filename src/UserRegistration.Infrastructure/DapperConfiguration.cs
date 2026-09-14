using System.Runtime.CompilerServices;
using Dapper;

namespace UserRegistration.Infrastructure;

internal static class DapperConfiguration
{
    [ModuleInitializer]
    internal static void Configure()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
}
