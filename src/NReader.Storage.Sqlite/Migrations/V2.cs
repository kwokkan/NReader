namespace NReader.Storage.Sqlite.Migrations;

internal static class V2
{
    internal readonly static string Sql = @"
alter table article
add column json_content text;
";
}
