using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using NReader.Storage.Abstractions;

namespace NReader.Storage.Sqlite;

public class SqliteStorageProvider : IStorageProvider
{
    private readonly ILogger<SqliteStorageProvider> _logger;
    private readonly SqliteOptions _options;

    private string? _connectionString;

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
        _connectionString = connectionStringBuilder.ConnectionString;

        using var connection = CreateConnection();

        await connection.OpenAsync();

        await CreateDatabaseAsync(connection);

        await MigrateDatabaseAsync(connection);
    }

    async Task IStorageProvider.ReadArticlesAsync(string userId, IEnumerable<string> articleIds)
    {
        using var connection = CreateConnection();

        await connection.OpenAsync();

        var uid = await GetOrCreateUserId(connection, userId);

        using var transaction = await connection.BeginTransactionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "insert into history (read_at_utc, user_id, article_id) values (@readAt, @userId, @articleId)";
        await command.PrepareAsync();

        var now = DateTime.UtcNow;

        foreach (var articleId in articleIds)
        {
            command.Parameters.Clear();

            command.Parameters.AddWithValue("@readAt", now);
            command.Parameters.AddWithValue("@userId", uid);
            command.Parameters.AddWithValue("@articleId", articleId);

            await command.ExecuteNonQueryAsync();
        }
    }

    private SqliteConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
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

    private static async Task<long> GetOrCreateUserId(SqliteConnection connection, string userId)
    {
        await connection.ExecuteNonQueryAsync(
            "insert or ignore into user (identifier, created_at_utc) values (@identifier, @now);",
            new SqliteParameter("@identifier", userId),
            new SqliteParameter("@now", DateTime.UtcNow)
        );

        return await connection.ExecuteScalarAsync<long>("select id from user where identifier = @identifier;");
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
