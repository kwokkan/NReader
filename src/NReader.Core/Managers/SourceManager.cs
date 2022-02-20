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

    async Task<IEnumerable<MappedSource>> ISourceManager.GetAllSourcesAsync()
    {
        var sources = await _sourceService.GetSourcesAsync();

        var sourceIds = sources.Select(x => x.Url.ToString()).ToArray();

        var mappedIds = await _storageProvider.GetOrCreateSourcesAsync(sourceIds);

        var mapped = new List<MappedSource>(sourceIds.Length);
        foreach (var source in sources)
        {
            mapped.Add(new MappedSource
            {
                Id = mappedIds.Single(x => x.Key == source.Url.ToString()).Value,
                Source = source,
            });
        }

        return mapped;
    }

    async Task<IReadOnlyCollection<MappedFeed>> ISourceManager.GetFeedsAsync(MappedSource source)
    {
        var feeds = await source.Source.GetFeedsAsync();

        var feedIds = feeds.Select(x => x.Id).ToArray();

        var mappedIds = await _storageProvider.GetOrCreateFeedsAsync(source.Id, feedIds);

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
}
