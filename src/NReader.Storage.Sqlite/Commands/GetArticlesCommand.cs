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

    internal async Task<Dictionary<long, string>> ExecuteAsync(
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

        GenerateAdditionalQuery(
            columns,
            additionalJoins,
            additionalWheres,
            groupBys,
            parameters,
            userKey,
            unread
        );

        var sql = SelectSql
            .Replace("{columns}", string.Join(", ", columns))
            .Replace("{additionalJoins}", string.Join("\n", additionalJoins))
            .Replace("{additionalWheres}", string.Join("\n", additionalWheres))
            .Replace("{groupBy}", groupBys.Count == 0 ? "" : "group by " + string.Join(", ", groupBys))
        ;

        await using var reader = await connection.ExecuteReaderAsync(sql, parameters.ToArray());

        var mapped = new Dictionary<long, string>();

        while (await reader.ReadAsync())
        {
            var newId = reader.GetInt64(0);
            var json = reader.GetString(1);

            mapped.Add(newId, json);
        }

        return mapped;
    }

    private static void GenerateAdditionalQuery(
        List<string> columns,
        List<string> additionalJoins,
        List<string> additionalWheres,
        List<string> groupBys,
        List<SqliteParameter> additionalParameters,
        long? userKey,
        bool? unread
    )
    {
        if (userKey.HasValue)
        {
            if (unread.HasValue)
            {
                groupBys.AddRange(columns);
                columns.Add("count(h.id) as count");
                additionalJoins.Add("left join history h on h.article_id = a.id and h.user_id = @userId");
                additionalWheres.Add($"and h.id is {(unread.Value ? "" : "not ")}null");
                additionalParameters.Add(new SqliteParameter("@userId", userKey.Value));
            }
        }
    }
}
