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

    Task<IReadOnlyCollection<StoredFeed>> StoreFeedsAsync(IStoredSourceId sourceId, IEnumerable<Feed> feeds);

    Task<IReadOnlyCollection<StoredArticle>> StoreArticlesAsync(IStoredFeedId feedId, IEnumerable<Article> articles);

    Task<IReadOnlyCollection<StoredArticle>> GetArticlesAsync(IStoredFeedId feedId);

    Task ReadArticlesAsync(string userId, IEnumerable<string> articleIds);
}
