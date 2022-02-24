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

    Task<IReadOnlyCollection<StoredFeed>> GetOrCreateFeedsAsync(IStoredSourceId sourceId, IEnumerable<Feed> feeds);

    Task<IReadOnlyCollection<StoredArticle>> StoreArticlesAsync(IStoredFeedId feedId, IEnumerable<Article> articles);

    Task ReadArticlesAsync(string userId, IEnumerable<string> articleIds);
}
