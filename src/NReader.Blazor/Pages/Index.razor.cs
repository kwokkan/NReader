using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

using NReader.Abstractions;

namespace NReader.Blazor.Pages
{
    public partial class Index
    {
        [CascadingParameter(Name = "SelectedSource")]
        private Source SelectedSource { get; set; }

        [CascadingParameter(Name = "SelectedFeed")]
        private Feed SelectedFeed { get; set; }

        [CascadingParameter(Name = "SelectedArticle")]
        private Article SelectedArticle { get; set; }

        private IReadOnlyCollection<Article> _articles;

        private async Task HandleArticleSelectedAsync(Article article)
        {
            SelectedArticle = await SelectedSource.GetArticleAsync(article);
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (SelectedFeed != null)
            {
                _articles = await SelectedSource.GetArticlesAsync(SelectedFeed);
            }
        }
    }
}
