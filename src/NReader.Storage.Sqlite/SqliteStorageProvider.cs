using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using NReader.Storage.Abstractions;

namespace NReader.Storage.Sqlite;

public class SqliteStorageProvider : IStorageProvider
{
    private readonly ILogger<SqliteStorageProvider> _logger;
    private readonly SqliteOptions _options;

    public SqliteStorageProvider(ILogger<SqliteStorageProvider> logger, SqliteOptions options)
    {
        _logger = logger;
        _options = options;
    }

    async Task IStorageProvider.InitialiseAsync()
    {
        if (string.IsNullOrWhiteSpace(_options.AppDb))
        {
            throw new Exception("AppDb cannot be empty.");
        }

        var absoluteDbPath = Path.IsPathFullyQualified(_options.AppDb) ? _options.AppDb : Path.Combine(AppContext.BaseDirectory, _options.AppDb);
        var fullPath = Path.GetDirectoryName(absoluteDbPath);

        Directory.CreateDirectory(fullPath!);

        var connectionStringBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = absoluteDbPath,
        };

        using var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

        await connection.OpenAsync();

        await CreateDatabaseAsync(connection);

        await MigrateDatabaseAsync(connection);

        await connection.CloseAsync();
    }

    private static async Task CreateDatabaseAsync(SqliteConnection connection)
    {
        var count = await connection.ExecuteScalarAsync<long>("select count(*) from sqlite_schema ss where ss.type = 'table' and ss.name = 'metadata'");

        if (count > 0)
        {
            return;
        }

        await connection.ExecuteNonQueryAsync("create table metadata (version integer)");

        await connection.ExecuteNonQueryAsync("insert into metadata(version) values (0)");
    }

    private async Task MigrateDatabaseAsync(SqliteConnection connection)
    {
        _logger.LogInformation("Migrating database.");

        var version = (int)await connection.ExecuteScalarAsync<long>("select version from metadata");

        for (var i = version; i < Scripts.Migrations.Count; i++)
        {
            _logger.LogInformation("Migrating version {Version}.", i + 1);

            var transaction = await connection.BeginTransactionAsync();

            await connection.ExecuteNonQueryAsync(Scripts.Migrations[i]);

            await connection.ExecuteNonQueryAsync($"update metadata set version = {i + 1}");

            await transaction.CommitAsync();

            _logger.LogInformation("Migrated version {Version}.", i + 1);
        }

        _logger.LogInformation("Finished migration.");
    }
}
