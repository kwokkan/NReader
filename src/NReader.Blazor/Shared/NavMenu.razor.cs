using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using NReader.Abstractions;
using NReader.Core;

namespace NReader.Blazor.Shared
{
    public partial class NavMenu
    {
        [Inject]
        private ISourceManager SourceManager { get; set; }

        [Parameter]
        public EventCallback<Source> OnSourceSelected { get; set; }

        [Parameter]
        public EventCallback<Feed> OnFeedSelected { get; set; }

        [Parameter]
        public EventCallback<Article> OnArticleSelected { get; set; }

        private bool collapseNavMenu = true;

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        private IEnumerable<Source> _sources;

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        private async Task HandleOnSourceSelectedAsync(Source source)
        {
            await OnSourceSelected.InvokeAsync(source);
        }

        private async Task HandleOnFeedSelectedAsync(Feed feed)
        {
            await OnFeedSelected.InvokeAsync(feed);
        }

        private async Task HandleOnArticleSelectedAsync(Article article)
        {
            await OnArticleSelected.InvokeAsync(article);
        }

        protected override async Task OnInitializedAsync()
        {
            _sources = await SourceManager.GetAllSourcesAsync();
        }
    }
}