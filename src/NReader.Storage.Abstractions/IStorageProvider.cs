namespace NReader.Storage.Abstractions;

public interface IStorageProvider
{
    Task InitialiseAsync();
}
