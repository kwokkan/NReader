using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

using NReader.Abstractions;
using NReader.Core;

namespace NReader.Blazor.Pages
{
    public partial class Index
    {
        [Inject]
        private ISourceService SourceService { get; set; }

        private IEnumerable<Source> Sources { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            Sources = await SourceService.GetSourcesAsync();
        }
    }
}
