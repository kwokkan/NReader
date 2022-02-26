using System.Collections.Generic;
using System.Threading.Tasks;
using NReader.Storage.Abstractions;

namespace NReader.Core;

public interface ISourceManager
{
    Task<IReadOnlyCollection<StoredSource>> GetAllSourcesAsync();

    Task<IReadOnlyCollection<StoredFeed>> GetFeedsAsync(StoredSource source);

    Task<StoredArticle> GetArticleAsync(StoredSource source, StoredArticle article);

    Task<IReadOnlyCollection<StoredArticle>> GetArticlesAsync(StoredSource source, StoredFeed feed, bool refresh, string userId, bool? unread = null);

    Task ReadArticlesAsync(string userId, IEnumerable<StoredArticle> articles);
}
