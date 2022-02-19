using NReader.Storage.Sqlite.Migrations;

namespace NReader.Storage.Sqlite;

internal static class Scripts
{
    internal static readonly IReadOnlyList<string> Migrations = new string[]
    {
        V1.Sql,
    };
}
