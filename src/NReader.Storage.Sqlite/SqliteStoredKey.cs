using NReader.Storage.Abstractions;

namespace NReader.Storage.Sqlite;

public class SqliteStoredKey : IStoredKey
{
    string IStoredKey.Value => Key.ToString();

    internal long Key { get; set; }

    internal SqliteStoredKey(long key)
    {
        Key = key;
    }
}
