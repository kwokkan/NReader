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
        public EventCallback<MappedSource> OnSourceSelected { get; set; }

        [Parameter]
        public EventCallback<MappedFeed> OnFeedSelected { get; set; }

        [Parameter]
        public EventCallback<MappedArticle> OnArticleSelected { get; set; }

        private bool collapseNavMenu = true;

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        private IEnumerable<MappedSource> _sources;

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        private async Task HandleOnSourceSelectedAsync(MappedSource source)
        {
            await OnSourceSelected.InvokeAsync(source);
        }

        private async Task HandleOnFeedSelectedAsync(MappedFeed feed)
        {
            await OnFeedSelected.InvokeAsync(feed);
        }

        private async Task HandleOnArticleSelectedAsync(MappedArticle article)
        {
            await OnArticleSelected.InvokeAsync(article);
        }

        protected override async Task OnInitializedAsync()
        {
            _sources = await SourceManager.GetAllSourcesAsync();
        }
    }
}