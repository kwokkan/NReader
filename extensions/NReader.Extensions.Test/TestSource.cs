using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NReader.Abstractions;

namespace NReader.Extensions.Test
{
    public class TestSource : Source
    {
        private static readonly IReadOnlyCollection<Article> TestArticles = new List<Article>
        {
            new Article
            {
                Id = "1",
                Title = "Chapter 1",
                Published = DateTime.UtcNow,
                Pages = new List<Page>
                {
                    new Page
                    {
                        Type = ArticleType.Text,
                        Content = "Hello world"
                    }
                }
            },
            new Article
            {
                Id = "2",
                Title = "Many pages",
                Published = DateTime.UtcNow,
                Pages = Enumerable.Range(0, 100)
                    .Select(x => new Page
                    {
                        Type = ArticleType.Text,
                        Content = "Page #" + x
                    }).ToList()
            }
        };

        private static readonly Pagination<Article> TestPagination = new Pagination<Article>
        {
            Offset = 0,
            Total = TestArticles.Count,
            Results = TestArticles,
        };

        private static readonly IReadOnlyCollection<Feed> TestFeeds = new Feed[]
        {
            new Feed
            {
                Articles = TestArticles
            }
        };

        public override string Title => "Test Source";

        public override Uri Url => new Uri("https://localhost/");

        public override Task<IReadOnlyCollection<Feed>> GetFeedsAsync()
        {
            return Task.FromResult(TestFeeds);
        }

        public override Task<Pagination<Article>> GetArticlesAsync(Feed feed, int offset = 0, int limit = 100)
        {
            return Task.FromResult(TestPagination);
        }

        public override Task<Article> GetArticleAsync(Article article)
        {
            return Task.FromResult(article);
        }
    }
}
