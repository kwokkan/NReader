using NReader.Storage.Abstractions;

namespace NReader.Storage.Sqlite;

public class SqliteStoredArticleId : SqliteStoredIdBase, IStoredArticleId
{
    internal SqliteStoredArticleId(long key)
        : base(key)
    {
    }
}
