using Microsoft.Data.Sqlite;

namespace NReader.Storage.Sqlite;

internal static class SqliteConnectionExtensions
{
    internal static async Task<int> ExecuteNonQueryAsync(this SqliteConnection connection, string sql, params SqliteParameter[] paramters)
    {
        var command = connection.CreateCommand();

        command.CommandText = sql;

        command.Parameters.AddRange(paramters);

        return await command.ExecuteNonQueryAsync();
    }

    internal static async Task<TReturn> ExecuteScalarAsync<TReturn>(this SqliteConnection connection, string sql, params SqliteParameter[] parameters)
    {
        var command = connection.CreateCommand();

        command.CommandText = sql;

        command.Parameters.AddRange(parameters);

        return (TReturn)await command.ExecuteScalarAsync();
    }
}
