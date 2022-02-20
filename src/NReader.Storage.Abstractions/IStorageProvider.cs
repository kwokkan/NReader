namespace NReader.Storage.Abstractions;

public interface IStorageProvider
{
    /// <summary>
    /// Initialise the storage.
    /// 
    /// Must be called before any other methods are used.
    /// </summary>
    /// <returns></returns>
    Task InitialiseAsync();

    Task<IDictionary<string, long>> GetOrCreateSourcesAsync(IEnumerable<string> sourceIds);

    Task ReadArticlesAsync(string userId, IEnumerable<string> articleIds);
}
