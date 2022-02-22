using NReader.Abstractions;

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

    Task<IReadOnlyCollection<StoredSource>> StoreSourcesAsync(IEnumerable<Source> sources);

    Task<IDictionary<string, long>> GetOrCreateFeedsAsync(IStoredKey storeId, IEnumerable<string> feedIds);

    Task<IDictionary<string, long>> GetOrCreateArticlesAsync(long feedId, IEnumerable<string> articleIds);

    Task ReadArticlesAsync(string userId, IEnumerable<string> articleIds);
}
