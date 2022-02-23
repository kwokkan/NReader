using System.Collections.Generic;
using System.Threading.Tasks;
using NReader.Storage.Abstractions;

namespace NReader.Core;

public interface ISourceManager
{
    Task<IReadOnlyCollection<StoredSource>> GetAllSourcesAsync();

    Task<IReadOnlyCollection<StoredFeed>> GetFeedsAsync(StoredSource source);

    Task<StoredArticle> GetArticleAsync(StoredSource source, StoredArticle article);

    Task<IReadOnlyCollection<StoredArticle>> GetArticlesAsync(StoredSource source, StoredFeed feed);

    Task ReadArticlesAsync(string userId, IEnumerable<StoredArticle> articles);
}
