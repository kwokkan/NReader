using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using NReader.Abstractions;
using NReader.Core;

namespace NReader.Blazor.Shared
{
    public partial class SourceNavItem
    {
        [Inject]
        private ISourceManager SourceManager { get; set; }

        [Parameter]
        public EventCallback<Source> OnSourceSelected { get; set; }

        [Parameter]
        public EventCallback<Feed> OnFeedSelected { get; set; }

        [Parameter]
        public EventCallback<Article> OnArticleSelected { get; set; }

        [Parameter]
        public MappedSource Source { get; set; }

        private IReadOnlyCollection<MappedFeed> _feeds;

        protected override async Task OnInitializedAsync()
        {
            _feeds = await SourceManager.GetFeedsAsync(Source);
        }

        private async Task HandleOnSourceSelectedAsync(MappedSource source)
        {
            await OnSourceSelected.InvokeAsync(source.Source);
        }

        private async Task HandleOnFeedSelectedAsync(MappedFeed feed)
        {
            await OnFeedSelected.InvokeAsync(feed.Feed);
        }

        private async Task HandleOnArticleSelectedAsync(Article article)
        {
            await OnArticleSelected.InvokeAsync(article);
        }
    }
}