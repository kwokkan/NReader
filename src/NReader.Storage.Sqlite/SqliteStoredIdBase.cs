namespace NReader.Storage.Sqlite;

public abstract class SqliteStoredIdBase 
{
    public string Value => Key.ToString();
    
    internal long Key { get; set; }

    internal SqliteStoredIdBase(long key)
    {
        Key = key;
    }
}
