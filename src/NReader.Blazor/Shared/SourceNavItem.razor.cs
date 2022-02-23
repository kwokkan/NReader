using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using NReader.Core;
using NReader.Storage.Abstractions;

namespace NReader.Blazor.Shared
{
    public partial class SourceNavItem
    {
        [Inject]
        private ISourceManager SourceManager { get; set; }

        [Parameter]
        public EventCallback<StoredSource> OnSourceSelected { get; set; }

        [Parameter]
        public EventCallback<StoredFeed> OnFeedSelected { get; set; }

        [Parameter]
        public EventCallback<StoredArticle> OnArticleSelected { get; set; }

        [Parameter]
        public StoredSource Source { get; set; }

        private IReadOnlyCollection<StoredFeed> _feeds;

        protected override async Task OnInitializedAsync()
        {
            _feeds = await SourceManager.GetFeedsAsync(Source);
        }

        private async Task HandleOnSourceSelectedAsync(StoredSource source)
        {
            await OnSourceSelected.InvokeAsync(source);
        }

        private async Task HandleOnFeedSelectedAsync(StoredFeed feed)
        {
            await OnFeedSelected.InvokeAsync(feed);
        }

        private async Task HandleOnArticleSelectedAsync(StoredArticle article)
        {
            await OnArticleSelected.InvokeAsync(article);
        }
    }
}