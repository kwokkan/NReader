using Microsoft.Extensions.Configuration;
using NReader.Storage.Abstractions;
using NReader.Storage.Sqlite;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqliteStorageProvider(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new SqliteOptions
        {
            AppDb = configuration.GetConnectionString("AppDb")
        };
        services.AddSingleton(options);

        services.AddScoped<IStorageProvider, SqliteStorageProvider>();

        return services;
    }
}
