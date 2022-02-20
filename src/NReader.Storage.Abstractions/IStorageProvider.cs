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

    Task<IDictionary<string, long>> GetOrCreateFeedsAsync(long sourceId, IEnumerable<string> feedIds);

    Task ReadArticlesAsync(string userId, IEnumerable<string> articleIds);
}
