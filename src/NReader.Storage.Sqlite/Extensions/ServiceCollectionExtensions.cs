using NReader.Storage.Abstractions;
using NReader.Storage.Sqlite;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqliteStorageProvider(this IServiceCollection services)
    {
        services.AddScoped<IStorageProvider, SqliteStorageProvider>();

        return services;
    }
}
