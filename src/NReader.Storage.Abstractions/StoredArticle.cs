using NReader.Abstractions;

namespace NReader.Storage.Abstractions;

public class StoredArticle
{
    public IStoredArticleId Id { get; init; }

    public Article Article { get; init; }

    public UserStats? UserStats { get; init; }

    public StoredArticle(IStoredArticleId id, Article article)
    {
        Id = id;
        Article = article;
    }
}
