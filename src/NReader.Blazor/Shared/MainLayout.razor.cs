using System.Threading.Tasks;
using NReader.Core;
using NReader.Storage.Abstractions;

namespace NReader.Blazor.Shared
{
    public partial class MainLayout
    {
        private StoredSource _selectedSource;
        private StoredFeed _selectedFeed;
        private StoredArticle _selectedArticle;

        private Task HandleOnSourceSelectedAsync(StoredSource source)
        {
            _selectedSource = source;
            _selectedArticle = null;

            return Task.CompletedTask;
        }

        private Task HandleOnFeedSelectedAsync(StoredFeed feed)
        {
            _selectedFeed = feed;

            return Task.CompletedTask;
        }

        private Task HandleOnArticleSelectedAsync(StoredArticle article)
        {
            _selectedArticle = article;

            return Task.CompletedTask;
        }
    }
}
