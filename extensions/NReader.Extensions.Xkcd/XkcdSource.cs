using AngleSharp.Html.Parser;
using NReader.Abstractions;

namespace NReader.Extensions.Xkcd;

public class XkcdSource : Source
{
    private const string FeedUrl = "https://xkcd.com/archive/";

    private static readonly IReadOnlyCollection<Feed> DefaultFeeds = new Feed[]
    {
        new Feed
        {
            Uri = new Uri(FeedUrl)
        }
    };

    public override string Title => "xkcd";

    public override Uri Url => new Uri("https://xkcd.com/");

    private readonly HttpClient _httpClient;

    public XkcdSource(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public override Task<IReadOnlyCollection<Feed>> GetFeedsAsync()
    {
        return Task.FromResult(DefaultFeeds);
    }

    public override async Task<IReadOnlyCollection<Article>> GetArticlesAsync(Feed feed)
    {
        var responseString = await _httpClient.GetStringAsync(feed.Uri);

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
