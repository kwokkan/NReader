using NReader.Storage.Abstractions;

namespace NReader.Storage.Sqlite;

public class SqliteStoredFeedId : SqliteStoredIdBase, IStoredFeedId
{
    internal SqliteStoredFeedId(long key)
        : base(key)
    {
    }
}
