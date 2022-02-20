using System.Collections.Generic;
using System.Threading.Tasks;

namespace NReader.Core;

public interface ISourceManager
{
    Task<IEnumerable<MappedSource>> GetAllSourcesAsync();

    Task<IReadOnlyCollection<MappedFeed>> GetFeedsAsync(MappedSource source);

    Task<IReadOnlyCollection<MappedArticle>> GetArticlesAsync(MappedSource source, MappedFeed feed);
}
