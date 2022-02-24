using NReader.Storage.Abstractions;

namespace NReader.Storage.Sqlite;

public class SqliteStoredSourceId : SqliteStoredIdBase, IStoredSourceId
{
    internal SqliteStoredSourceId(long key)
        : base(key)
    {
    }
}
