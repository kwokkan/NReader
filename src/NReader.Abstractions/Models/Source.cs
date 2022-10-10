using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NReader.Abstractions
{
    public abstract class Source
    {
        private static readonly IReadOnlyCollection<Feed> EmptyFeeds = Array.Empty<Feed>();
        private static readonly Pagination<Article> EmptyArticles = new Pagination<Article> { Results = Array.Empty<Article>() };

        public virtual string Title { get; }

        public virtual Uri Url { get; }

        public virtual Task<IReadOnlyCollection<Feed>> GetFeedsAsync()
        {
            return Task.FromResult(EmptyFeeds);
        }

        public virtual Task<Pagination<Article>> GetArticlesAsync(Feed feed, int offset = 0, int limit = 100)
        {
            return Task.FromResult(EmptyArticles);
        }

        public virtual Task<Article> GetArticleAsync(Article article)
        {
            return Task.FromResult(Article.Empty);
        }
    }
}
