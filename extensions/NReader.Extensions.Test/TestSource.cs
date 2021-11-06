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
                Pages = Enumerable.Range(0, 100)
                    .Select(x => new Page
                    {
                        Type = ArticleType.Text,
                        Content = "Page #" + x
                    }).ToList()
            }
        };

        public override string Title => "Test Source";

        public override Uri Url => new Uri("https://localhost/");

        public override Task<IReadOnlyCollection<Article>> GetArticlesAsync()
        {
            return Task.FromResult(TestArticles);
        }
    }
}
