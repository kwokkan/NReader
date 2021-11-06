using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using NReader.Abstractions;

namespace NReader.Blazor.Shared
{
    public partial class SourceNavItem
    {
        [Parameter]
        public EventCallback<Source> OnSourceSelected { get; set; }

        [Parameter]
        public EventCallback<Article> OnArticleSelected { get; set; }

        [Parameter]
        public Source Source { get; set; }

        private IReadOnlyCollection<Article> _articles;

        protected override async Task OnInitializedAsync()
        {
            _articles = await Source.GetArticlesAsync();
        }

        private async Task HandleOnSourceSelectedAsync(Source source)
        {
            await OnSourceSelected.InvokeAsync(source);
        }

        private async Task HandleOnArticleSelectedAsync(Article article)
        {
            await OnArticleSelected.InvokeAsync(article);
        }
    }
}