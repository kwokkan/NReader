using Microsoft.AspNetCore.Components;

using NReader.Abstractions;

namespace NReader.Blazor.Pages
{
    public partial class Index
    {
        [CascadingParameter(Name = "SelectedSource")]
        private Source SelectedSource { get; set; }

        [CascadingParameter(Name = "SelectedArticle")]
        private Article SelectedArticle { get; set; }
    }
}
