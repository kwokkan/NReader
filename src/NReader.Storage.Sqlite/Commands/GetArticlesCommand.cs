using Microsoft.Data.Sqlite;

namespace NReader.Storage.Sqlite;

internal class GetArticlesCommand
{
    private const string SelectSql = @"
select {columns}
from article a
{additionalJoins}
where a.feed_id = @feedId
{additionalWheres}
{groupBy}
order by a.created_at_utc desc
;
";

    internal class ExecuteResult
    {
        internal long Key { get; init; }

        internal string JsonContent { get; init; } = default!;

        internal long ReadCount { get; init; }
    }

    internal class ExecuteReturn
    {
        internal IReadOnlyCollection<ExecuteResult> Results { get; init; } = default!;

        internal bool HasUserStats { get; init; }
    }

    internal async Task<ExecuteReturn> ExecuteAsync(
        SqliteConnection connection,
        long feedKey,
        long? userKey,
        bool? unread
    )
    {
        var columns = new List<string>
        {
            "a.id",
            "a.json_content"
        };
        var additionalJoins = new List<string>();
        var additionalWheres = new List<string>();
        var groupBys = new List<string>();
        var parameters = new List<SqliteParameter>
        {
            new SqliteParameter("@feedId", feedKey),
        };

        bool hasUserStats;
        GenerateAdditionalQuery(
            columns,
            additionalJoins,
            additionalWheres,
            groupBys,
            parameters,
            userKey,
            unread,
            out hasUserStats
        );

        var sql = SelectSql
            .Replace("{columns}", string.Join(", ", columns))
            .Replace("{additionalJoins}", string.Join("\n", additionalJoins))
            .Replace("{additionalWheres}", string.Join("\n", additionalWheres))
            .Replace("{groupBy}", groupBys.Count == 0 ? "" : "group by " + string.Join(", ", groupBys))
        ;

        await using var reader = await connection.ExecuteReaderAsync(sql, parameters.ToArray());

        var results = new List<ExecuteResult>();

        while (await reader.ReadAsync())
        {
            var newId = reader.GetInt64(0);
            var json = reader.GetString(1);

            var result = new ExecuteResult
            {
                Key = newId,
                JsonContent = json,
                ReadCount = hasUserStats ? reader.GetInt64(2) : default,
            };

            results.Add(result);
        }

        return new ExecuteReturn
        {
            Results = results,
            HasUserStats = hasUserStats,
        };
    }

    private static void GenerateAdditionalQuery(
        List<string> columns,
        List<string> additionalJoins,
        List<string> additionalWheres,
        List<string> groupBys,
        List<SqliteParameter> additionalParameters,
        long? userKey,
        bool? unread,
        out bool hasUserStats
    )
    {
        var hasUserColumns = false;

        if (userKey.HasValue)
        {
            hasUserColumns = true;

            groupBys.AddRange(columns);
            columns.Add("count(h.id) as count");
            additionalJoins.Add("left join history h on h.article_id = a.id and h.user_id = @userId");
            additionalParameters.Add(new SqliteParameter("@userId", userKey.Value));

            if (unread.HasValue)
            {
                additionalWheres.Add($"and h.id is {(unread.Value ? "" : "not ")}null");
            }
        }

        hasUserStats = hasUserColumns;
    }
}
