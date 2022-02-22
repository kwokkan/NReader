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
        private MappedArticle SelectedArticle { get; set; }

        private IReadOnlyCollection<MappedArticle> _articles;

        private async Task HandleArticleSelectedAsync(MappedArticle article)
        {
            SelectedArticle = await SourceManager.GetArticleAsync(SelectedSource, article);

            await SourceManager.ReadArticlesAsync(AuthenticationConstants.UserId, new[] { SelectedArticle });
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (SelectedFeed != null)
            {
                _articles = await SourceManager.GetArticlesAsync(SelectedSource, SelectedFeed);
            }
        }
    }
}
