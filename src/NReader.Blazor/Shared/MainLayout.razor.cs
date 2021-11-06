using System.Threading.Tasks;
using NReader.Abstractions;

namespace NReader.Blazor.Shared
{
    public partial class MainLayout
    {
        private Task HandleOnSourceSelectedAsync(Source source)
        {
            return Task.CompletedTask;
        }

        private Task HandleOnArticleSelectedAsync(Article article)
        {
            return Task.CompletedTask;
        }
    }
}
