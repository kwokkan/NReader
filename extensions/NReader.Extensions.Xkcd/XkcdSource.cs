using System.Text.Json;
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

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

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
                Uri = new Uri(Url, $"{id}/info.0.json"),
            });
        }

        return articles;
    }

    public override async Task<Article> GetArticleAsync(Article article)
    {
        var responseString = await _httpClient.GetStringAsync(article.Uri);
        var model = JsonSerializer.Deserialize<InternalArticle>(responseString, _jsonSerializerOptions);

        var output = new Article
        {
            Id = article.Id,
            Title = model.Title,
            Uri = article.Uri,
            Published = new DateTime(model.Year, model.Month, model.Day),
            Pages = new Page[]
            {
                new Page
                {
                    Content  = model.Img,
                    Type = ArticleType.Image,
                },
                new Page
                {
                    Content = model.Alt,
                }
            }
        };

        return output;
    }
}
