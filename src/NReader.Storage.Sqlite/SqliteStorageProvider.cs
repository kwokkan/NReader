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
        using var connection = CreateConnection();

        await connection.OpenAsync();

        await CreateDatabaseAsync(connection);

        await MigrateDatabaseAsync(connection);
    }

    async Task<IDictionary<string, long>> IStorageProvider.GetOrCreateSourcesAsync(IEnumerable<string> sourceIds)
    {
        await using var connection = CreateConnection();

        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        var tempTableName = "tmp_" + Guid.NewGuid().ToString().Replace("-", string.Empty).ToLower();
        
        await connection.ExecuteNonQueryAsync($"create temp table {tempTableName} (identifier text unique not null);");
        
        var insertCommand = connection.CreateCommand();
        insertCommand.CommandText = $"insert or ignore into source (identifier, created_at_utc) values (@identifier, @now); insert into {tempTableName} values (@identifier);";

        var identifierParam = insertCommand.CreateParameter();
        identifierParam.ParameterName = "@identifier";
        insertCommand.Parameters.Add(identifierParam);

        var now = DateTime.UtcNow;
        var nowParam = insertCommand.CreateParameter();
        nowParam.ParameterName = "@now";
        nowParam.Value = now;
        insertCommand.Parameters.Add(nowParam);

        await insertCommand.PrepareAsync();

        foreach (var sourceId in sourceIds)
        {
            identifierParam.Value = sourceId;
            await insertCommand.ExecuteNonQueryAsync();
        }

        var selectCommand = connection.CreateCommand();
        selectCommand.CommandText = $"select s.id, s.identifier from source s inner join {tempTableName} t on t.identifier = s.identifier;";

        await using var reader = await selectCommand.ExecuteReaderAsync();
        var mapped = new Dictionary<string, long>();

        while (await reader.ReadAsync())
        {
            var newId = reader.GetInt64(0);
            var newIdentifier = reader.GetString(1);
              
            mapped.Add(newIdentifier, newId);
        }

        await transaction.CommitAsync();

        return mapped;
    }

    async Task<IDictionary<string, long>> IStorageProvider.GetOrCreateFeedsAsync(long sourceId, IEnumerable<string> feedIds)
    {
        await using var connection = CreateConnection();

        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        var tempTableName = "tmp_" + Guid.NewGuid().ToString().Replace("-", string.Empty).ToLower();

        await connection.ExecuteNonQueryAsync($"create temp table {tempTableName} (identifier text unique not null);");

        var insertCommand = connection.CreateCommand();
        insertCommand.CommandText = $"insert or ignore into feed (identifier, created_at_utc, source_id) values (@identifier, @now, @sourceId); insert into {tempTableName} values (@identifier);";

        var identifierParam = insertCommand.CreateParameter();
        identifierParam.ParameterName = "@identifier";
        insertCommand.Parameters.Add(identifierParam);

        var now = DateTime.UtcNow;
        var nowParam = insertCommand.CreateParameter();
        nowParam.ParameterName = "@now";
        nowParam.Value = now;
        insertCommand.Parameters.Add(nowParam);

        var sourceParam = insertCommand.CreateParameter();
        sourceParam.ParameterName = "@sourceId";
        sourceParam.Value = sourceId;
        insertCommand.Parameters.Add(sourceParam);

        await insertCommand.PrepareAsync();

        foreach (var feedId in feedIds)
        {
            identifierParam.Value = feedId;
            await insertCommand.ExecuteNonQueryAsync();
        }

        var selectCommand = connection.CreateCommand();
        selectCommand.CommandText = $"select f.id, f.identifier from feed f inner join {tempTableName} t on t.identifier = f.identifier and f.source_id = @sourceId;";
        selectCommand.Parameters.AddWithValue("@sourceId", sourceId);

        await using var reader = await selectCommand.ExecuteReaderAsync();
        var mapped = new Dictionary<string, long>();

        while (await reader.ReadAsync())
        {
            var newId = reader.GetInt64(0);
            var newIdentifier = reader.GetString(1);

            mapped.Add(newIdentifier, newId);
        }

        await transaction.CommitAsync();

        return mapped;
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

        return new SqliteConnection(connectionStringBuilder.ConnectionString);
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
