using System.Threading.Tasks;
using NReader.Core;

namespace NReader.Blazor.Shared
{
    public partial class MainLayout
    {
        private MappedSource _selectedSource;
        private MappedFeed _selectedFeed;
        private MappedArticle _selectedArticle;

        private Task HandleOnSourceSelectedAsync(MappedSource source)
        {
            _selectedSource = source;
            _selectedArticle = null;

            return Task.CompletedTask;
        }

        private Task HandleOnFeedSelectedAsync(MappedFeed feed)
        {
            _selectedFeed = feed;

            return Task.CompletedTask;
        }

        private Task HandleOnArticleSelectedAsync(MappedArticle article)
        {
            _selectedArticle = article;

            return Task.CompletedTask;
        }
    }
}
