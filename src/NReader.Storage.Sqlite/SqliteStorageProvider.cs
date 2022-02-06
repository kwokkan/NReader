using NReader.Storage.Abstractions;

namespace NReader.Storage.Sqlite;

public class SqliteStorageProvider : IStorageProvider
{
    Task IStorageProvider.InitialiseAsync()
    {
        throw new NotImplementedException();
    }
}
