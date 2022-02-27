using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NReader.Storage.Abstractions;

namespace NReader.Core;

public class SourceManager : ISourceManager
{
    private readonly ISourceService _sourceService;
    private readonly IStorageProvider _storageProvider;

    public SourceManager(ISourceService sourceService, IStorageProvider storageProvider)
    {
        _sourceService = sourceService;
        _storageProvider = storageProvider;
    }

    async Task<IReadOnlyCollection<StoredSource>> ISourceManager.GetAllSourcesAsync()
    {
        var sources = await _sourceService.GetSourcesAsync();

        var storedSources = await _storageProvider.StoreSourcesAsync(sources);

        return storedSources;
    }

    async Task<StoredArticle> ISourceManager.GetArticleAsync(StoredSource source, StoredArticle article)
    {
        var newArticle = await source.Source.GetArticleAsync(article.Article);

        return new StoredArticle(article.Id, newArticle);
    }

    async Task<IReadOnlyCollection<StoredArticle>> ISourceManager.GetArticlesAsync(StoredSource source, StoredFeed feed, bool refresh, string userId, bool? unread)
    {
        IReadOnlyCollection<StoredArticle> storedArticles;

        if (refresh)
        {
            var articles = await source.Source.GetArticlesAsync(feed.Feed);

            await _storageProvider.StoreArticlesAsync(feed.Id, articles);
        }

        var filter = new GetArticlesSearchFilter
        {
            UserId = userId,
            Unread = unread,
        };
        storedArticles = await _storageProvider.GetArticlesAsync(feed.Id, filter: filter);

        return storedArticles;
    }

    async Task<IReadOnlyCollection<StoredFeed>> ISourceManager.GetFeedsAsync(StoredSource source)
    {
        var feeds = await source.Source.GetFeedsAsync();

        var storedFeeds = await _storageProvider.StoreFeedsAsync(source.Id, feeds);

        return storedFeeds;
    }

    async Task ISourceManager.ReadArticlesAsync(string userId, IEnumerable<StoredArticle> articles)
    {
        var articleIds = articles.Select(x => x.Id).ToArray();

        await _storageProvider.ReadArticlesAsync(userId, articleIds);
    }
}
