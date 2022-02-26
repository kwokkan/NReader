using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using NReader.Abstractions;
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

    async Task<IReadOnlyCollection<StoredSource>> IStorageProvider.StoreSourcesAsync(IEnumerable<Source> sources)
    {
        await using var connection = CreateConnection();

        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        var tempTableName = "tmp_" + Guid.NewGuid().ToString().Replace("-", string.Empty).ToLower();

        await connection.ExecuteNonQueryAsync($"create temp table {tempTableName} (identifier text unique not null);");

        var insertCommand = connection.CreateCommand();
        insertCommand.CommandText = $"insert or ignore into source (identifier, created_at_utc) values (@identifier, @now); insert into {tempTableName} values (@identifier);";

        var now = DateTime.UtcNow;
        insertCommand.Parameters.AddWithValue("@now", now);

        var identifierParam = insertCommand.CreateParameter();
        identifierParam.ParameterName = "@identifier";
        insertCommand.Parameters.Add(identifierParam);

        await insertCommand.PrepareAsync();

        foreach (var source in sources)
        {
            identifierParam.Value = source.Url.ToString();
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

        var stored = new List<StoredSource>(mapped.Count);

        foreach (var source in sources)
        {
            stored.Add(
                new StoredSource(
                    new SqliteStoredSourceId(mapped.First(x => x.Key == source.Url.ToString()).Value),
                    source
                )
            );
        }

        return stored;
    }

    async Task<IReadOnlyCollection<StoredFeed>> IStorageProvider.StoreFeedsAsync(IStoredSourceId sourceId, IEnumerable<Feed> feeds)
    {
        var sourceKey = ((SqliteStoredIdBase)sourceId).Key;

        await using var connection = CreateConnection();

        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        var tempTableName = "tmp_" + Guid.NewGuid().ToString().Replace("-", string.Empty).ToLower();

        await connection.ExecuteNonQueryAsync($"create temp table {tempTableName} (identifier text unique not null);");

        var insertCommand = connection.CreateCommand();
        insertCommand.CommandText = $"insert or ignore into feed (identifier, created_at_utc, source_id) values (@identifier, @now, @sourceId); insert into {tempTableName} values (@identifier);";

        var now = DateTime.UtcNow;
        insertCommand.Parameters.AddWithValue("@now", now);
        insertCommand.Parameters.AddWithValue("@sourceId", sourceKey);

        var identifierParam = insertCommand.CreateParameter();
        identifierParam.ParameterName = "@identifier";
        insertCommand.Parameters.Add(identifierParam);

        await insertCommand.PrepareAsync();

        foreach (var feed in feeds)
        {
            identifierParam.Value = feed.Id;
            await insertCommand.ExecuteNonQueryAsync();
        }

        var selectCommand = connection.CreateCommand();
        selectCommand.CommandText = $"select f.id, f.identifier from feed f inner join {tempTableName} t on t.identifier = f.identifier and f.source_id = @sourceId;";
        selectCommand.Parameters.AddWithValue("@sourceId", sourceKey);

        await using var reader = await selectCommand.ExecuteReaderAsync();
        var mapped = new Dictionary<string, long>();

        while (await reader.ReadAsync())
        {
            var newId = reader.GetInt64(0);
            var newIdentifier = reader.GetString(1);

            mapped.Add(newIdentifier, newId);
        }

        await transaction.CommitAsync();

        var stored = new List<StoredFeed>(mapped.Count);

        foreach (var feed in feeds)
        {
            stored.Add(
                new StoredFeed(
                    new SqliteStoredFeedId(mapped.First(x => x.Key == feed.Id).Value),
                    feed
                )
            );
        }

        return stored;
    }

    async Task<IReadOnlyCollection<StoredArticle>> IStorageProvider.StoreArticlesAsync(IStoredFeedId feedId, IEnumerable<Article> articles)
    {
        var feedKey = ((SqliteStoredIdBase)feedId).Key;

        await using var connection = CreateConnection();

        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        var tempTableName = "tmp_" + Guid.NewGuid().ToString().Replace("-", string.Empty).ToLower();

        await connection.ExecuteNonQueryAsync($"create temp table {tempTableName} (identifier text unique not null);");

        var insertCommand = connection.CreateCommand();
        insertCommand.CommandText = @$"
insert or ignore into article (identifier, created_at_utc, feed_id, json_content) 
values (@identifier, @now, @feedId, @jsonContent);

insert into {tempTableName} values (@identifier);";

        var now = DateTime.UtcNow;
        insertCommand.Parameters.AddWithValue("@now", now);
        insertCommand.Parameters.AddWithValue("@feedId", feedKey);

        var identifierParam = insertCommand.CreateParameter();
        identifierParam.ParameterName = "@identifier";
        insertCommand.Parameters.Add(identifierParam);

        var jsonContentParam = insertCommand.CreateParameter();
        jsonContentParam.ParameterName = "@jsonContent";
        insertCommand.Parameters.Add(jsonContentParam) ;

        await insertCommand.PrepareAsync();

        foreach (var article in articles)
        {
            identifierParam.Value = article.Id;
            jsonContentParam.Value = JsonSerializer.Serialize(article);
            await insertCommand.ExecuteNonQueryAsync();
        }

        await using var reader = await connection.ExecuteReaderAsync(
            $"select a.id, a.identifier from article a inner join {tempTableName} t on t.identifier = a.identifier and a.feed_id = @feedId;",
            new SqliteParameter("@feedId", feedKey));

        var mapped = new Dictionary<string, long>();

        while (await reader.ReadAsync())
        {
            var newId = reader.GetInt64(0);
            var newIdentifier = reader.GetString(1);

            mapped.Add(newIdentifier, newId);
        }

        await transaction.CommitAsync();

        var stored = new List<StoredArticle>(mapped.Count);

        foreach (var article in articles)
        {
            stored.Add(
                new StoredArticle(
                    new SqliteStoredArticleId(mapped.First(x => x.Key == article.Id).Value),
                    article
                )
            );
        }

        return stored;
    }

    async Task<IReadOnlyCollection<StoredArticle>> IStorageProvider.GetArticlesAsync(IStoredFeedId feedId, GetArticlesSearchFilter? filter)
    {
        var feedKey = ((SqliteStoredIdBase)feedId).Key;
        long? userKey = null;

        await using var connection = CreateConnection();

        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        if (!string.IsNullOrWhiteSpace(filter?.UserId))
        {
            userKey = await GetOrCreateUserId(connection, filter.UserId);
        }

        var mapped = await new GetArticlesCommand().ExecuteAsync(
            connection,
            feedKey,
            userKey,
            filter?.Unread
        );

        await transaction.CommitAsync();

        var stored = new List<StoredArticle>(mapped.Count);

        foreach (var map in mapped)
        {
            stored.Add(
                new StoredArticle(
                    new SqliteStoredArticleId(map.Key),
                    JsonSerializer.Deserialize<Article>(map.Value)
                )
            );
        }

        return stored;
    }

    async Task IStorageProvider.ReadArticlesAsync(string userId, IEnumerable<IStoredArticleId> articleIds)
    {
        await using var connection = CreateConnection();

        await connection.OpenAsync();

        var uid = await GetOrCreateUserId(connection, userId);

        await using var transaction = await connection.BeginTransactionAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "insert into history (read_at_utc, user_id, article_id) values (@readAt, @userId, @articleId)";

        var now = DateTime.UtcNow;
        command.Parameters.AddWithValue("@readAt", now);
        command.Parameters.AddWithValue("@userId", uid);

        var articleParam = command.CreateParameter();
        articleParam.ParameterName = "@articleId";
        command.Parameters.Add(articleParam);

        await command.PrepareAsync();

        foreach (var articleId in articleIds)
        {
            articleParam.Value = ((SqliteStoredArticleId)articleId).Key;

            await command.ExecuteNonQueryAsync();
        }

        await transaction.CommitAsync();
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

        return await connection.ExecuteScalarAsync<long>(
            "select id from user where identifier = @identifier;",
            new SqliteParameter ("@identifier", userId)
        );
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
