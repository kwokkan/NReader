using System.Threading.Tasks;
using NReader.Abstractions;

namespace NReader.Blazor.Shared
{
    public partial class MainLayout
    {
        private Source _selectedSource;
        private Feed _selectedFeed;
        private Article _selectedArticle;

        private Task HandleOnSourceSelectedAsync(Source source)
        {
            _selectedSource = source;
            _selectedArticle = null;

            return Task.CompletedTask;
        }

        private Task HandleOnFeedSelectedAsync(Feed feed)
        {
            _selectedFeed = feed;

            return Task.CompletedTask;
        }

        private Task HandleOnArticleSelectedAsync(Article article)
        {
            _selectedArticle = article;

            return Task.CompletedTask;
        }
    }
}
