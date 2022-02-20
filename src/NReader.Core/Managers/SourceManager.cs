using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NReader.Abstractions;
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

    async Task<IEnumerable<Source>> ISourceManager.GetAllSourcesAsync()
    {
        var sources = await _sourceService.GetSourcesAsync();

        var sourceIds = sources.Select(x => x.Url.ToString()).ToArray();

        await _storageProvider.GetOrCreateSourcesAsync(sourceIds);

        return sources;
    }
}
