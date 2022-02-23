using NReader.Abstractions;

namespace NReader.Storage.Abstractions;

public class StoredArticle
{
    public IStoredKey Key { get; init; }

    public Article Article { get; init; }

    public StoredArticle(IStoredKey key, Article article)
    {
        Key = key;
        Article = article;
    }
}
