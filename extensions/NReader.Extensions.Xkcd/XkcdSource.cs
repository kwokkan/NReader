using AngleSharp.Html.Parser;
using NReader.Abstractions;

namespace NReader.Extensions.Xkcd;

public class XkcdSource : Source
{
    public override string Title => "xkcd";

    public override Uri Url => new Uri("https://xkcd.com/");

    private const string FeedUrl = "https://xkcd.com/archive/";

    private readonly HttpClient _httpClient;

    public XkcdSource(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public override async Task<IReadOnlyCollection<Article>> GetArticlesAsync()
    {
        var responseString = await _httpClient.GetStringAsync(FeedUrl);

        var parser = new HtmlParser();
        using var document = parser.ParseDocument(responseString);

        var elements = document.QuerySelectorAll("#middleContainer a");

        var articles = new List<Article>(elements.Length);

        foreach (var current in elements)
        {
            var href = current.Attributes["href"]!.Value;
            var id = href!.Replace("/", string.Empty);

            articles.Add(new Article
            {
                Id = id,
                Title = current.TextContent,
                Uri = new Uri(Url, href),
            });
        }

        return articles;
    }
}
