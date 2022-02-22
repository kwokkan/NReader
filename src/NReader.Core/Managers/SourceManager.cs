﻿using System.Collections.Generic;
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

    async Task<MappedArticle> ISourceManager.GetArticleAsync(StoredSource source, MappedArticle article)
    {
        var newArticle = await source.Source.GetArticleAsync(article.Article);

        return new MappedArticle
        {
            Id = article.Id,
            Article = newArticle
        };
    }

    async Task<IReadOnlyCollection<MappedArticle>> ISourceManager.GetArticlesAsync(StoredSource source, MappedFeed feed)
    {
        var articles = await source.Source.GetArticlesAsync(feed.Feed);

        var articleIds = articles.Select(x => x.Id).ToArray();

        var mappedIds = await _storageProvider.GetOrCreateArticlesAsync(feed.Id, articleIds);

        var mapped = new List<MappedArticle>(articleIds.Length);
        foreach (var article in articles)
        {
            mapped.Add(new MappedArticle
            {
                Id = mappedIds.Single(x => x.Key == article.Id).Value,
                Article = article,
            });
        }

        return mapped;
    }

    async Task<IReadOnlyCollection<MappedFeed>> ISourceManager.GetFeedsAsync(StoredSource source)
    {
        var feeds = await source.Source.GetFeedsAsync();

        var feedIds = feeds.Select(x => x.Id).ToArray();

        var mappedIds = await _storageProvider.GetOrCreateFeedsAsync(source.Key, feedIds);

        var mapped = new List<MappedFeed>(feedIds.Length);
        foreach (var feed in feeds)
        {
            mapped.Add(new MappedFeed
            {
                Id = mappedIds.Single(x => x.Key == feed.Id).Value,
                Feed = feed,
            });
        }

        return mapped;
    }

    async Task ISourceManager.ReadArticlesAsync(string userId, IEnumerable<MappedArticle> articles)
    {
        var articleIds = articles.Select(x => x.Article.Id).ToArray();

        await _storageProvider.ReadArticlesAsync(userId, articleIds);
    }
}
