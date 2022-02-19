using Microsoft.Data.Sqlite;

namespace NReader.Storage.Sqlite;

internal static class SqliteConnectionExtensions
{
    internal static async Task<int> ExecuteNonQueryAsync(this SqliteConnection connection, string sql)
    {
        var command = connection.CreateCommand();

        command.CommandText = sql;

        return await command.ExecuteNonQueryAsync();
    }

    internal static async Task<TReturn> ExecuteScalarAsync<TReturn>(this SqliteConnection connection, string sql)
    {
        var command = connection.CreateCommand();

        command.CommandText = sql;

        return (TReturn)await command.ExecuteScalarAsync();
    }
}
