using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using NReader.Blazor.Constants;
using NReader.Core;
using NReader.Storage.Abstractions;

namespace NReader.Blazor.Pages
{
    public partial class Index
    {
        [Inject]
        private ISourceManager SourceManager { get; set; }

        [CascadingParameter(Name = "SelectedSource")]
        private StoredSource SelectedSource { get; set; }

        [CascadingParameter(Name = "SelectedFeed")]
        private StoredFeed SelectedFeed { get; set; }

        [CascadingParameter(Name = "SelectedArticle")]
        private StoredArticle SelectedArticle { get; set; }

        private IReadOnlyCollection<StoredArticle> _articles;

        private bool? _unread = true;

        private async Task HandleArticleSelectedAsync(StoredArticle article)
        {
            SelectedArticle = await SourceManager.GetArticleAsync(SelectedSource, article);

            await SourceManager.ReadArticlesAsync(AuthenticationConstants.UserId, new[] { SelectedArticle });
        }

        private async Task HandleRefreshArticlesAsync()
        {
            _articles = await SourceManager.GetArticlesAsync(SelectedSource, SelectedFeed, true, AuthenticationConstants.UserId, unread: _unread);
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (SelectedFeed != null)
            {
                _articles = await SourceManager.GetArticlesAsync(SelectedSource, SelectedFeed, false, AuthenticationConstants.UserId, unread: _unread);
            }
        }
    }
}
