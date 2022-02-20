using System.Collections.Generic;
using System.Threading.Tasks;

namespace NReader.Core;

public interface ISourceManager
{
    Task<IEnumerable<MappedSource>> GetAllSourcesAsync();

    Task<IReadOnlyCollection<MappedFeed>> GetFeedsAsync(MappedSource source);

    Task<MappedArticle> GetArticleAsync(MappedSource source, MappedArticle article);

    Task<IReadOnlyCollection<MappedArticle>> GetArticlesAsync(MappedSource source, MappedFeed feed);

    Task ReadArticlesAsync(string userId, IEnumerable<MappedArticle> articles);
}
