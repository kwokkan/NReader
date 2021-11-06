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
        private ISourceService SourceService { get; set; }

        [Parameter]
        public EventCallback<Source> OnSourceSelected { get; set; }

        private bool collapseNavMenu = true;

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        private IEnumerable<Source> _sources;

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        private async Task HandleOnSourceClickAsync(Source source)
        {
            await OnSourceSelected.InvokeAsync(source);
        }

        protected override async Task OnInitializedAsync()
        {
            _sources = await SourceService.GetSourcesAsync();
        }
    }
}